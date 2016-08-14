using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Core
{
    using System.Collections;
    using System.Diagnostics;

    /// <summary>
    /// Dictionary designed to hold small number of items.
    /// Compared to the regular Dictionary, average overhead per-item is roughly the same, but 
    /// unlike regular dictionary, this one is based on an AVL tree and as such does not require 
    /// rehashing when items are added.
    /// It does require rebalancing, but that is allocation-free.
    ///
    /// Major caveats:
    ///  1) There is no Remove method. (can be added, but we do not seem to use Remove that much)
    ///  2) foreach [keys|values|pairs] may allocate a small array.
    ///  3) Performance is no longer O(1). At a certain count it becomes slower than regular Dictionary.
    ///     In comparison to regular Dictionary on my machine:
    ///        On trivial number of elements (5 or so) it is more than 2x faster.
    ///        The break even count is about 120 elements for read and 55 for write operations (with unknown initial size).
    ///        At UShort.MaxValue elements, this dictionary is 6x slower to read and 4x slower to write
    ///
    /// Generally, this dictionary is a win if number of elements is small, not known beforehand or both.
    ///
    /// If the size of the dictionary is known at creation and it is likely to contain more than 10 elements, 
    /// then regular Dictionary is a better choice.
    /// </summary>
    public class SmallDictionary<TK, TV> : IEnumerable<KeyValuePair<TK, TV>>, IDictionary<TK, TV>
    {
        private AvlNode root;
        private readonly IEqualityComparer<TK> comparer;

        public static readonly SmallDictionary<TK, TV> Empty = new SmallDictionary<TK, TV>(null);

        public SmallDictionary() : this(EqualityComparer<TK>.Default) { }

        public SmallDictionary(IEqualityComparer<TK> comparer)
        {
            this.comparer = comparer;
        }

        public SmallDictionary(SmallDictionary<TK, TV> other, IEqualityComparer<TK> comparer)
            : this(comparer)
        {
            // TODO: if comparers are same (often they are), then just need to clone the tree.
            foreach (var kv in other)
            {
                this.Add(kv.Key, kv.Value);
            }
        }

        private bool CompareKeys(TK k1, TK k2)
        {
            return this.comparer.Equals(k1, k2);
        }

        private int GetHashCode(TK tk)
        {
            return this.comparer.GetHashCode(tk);
        }

        public bool TryGetValue(TK key, out TV value)
        {
            if (this.root != null)
            {
                return this.TryGetValue(this.GetHashCode(key), key, out value);
            }

            value = default(TV);
            return false;
        }

        public void Add(TK key, TV value)
        {
            this.Insert(this.GetHashCode(key), key, value, add: true);
        }

        public TV this[TK key]
        {
            get
            {
                TV value;
                if (!this.TryGetValue(key, out value))
                {
                    throw new InvalidOperationException("key not found");
                }

                return value;
            }

            set
            {
                this.Insert(this.GetHashCode(key), key, value, add: false);
            }
        }

        public bool ContainsKey(TK key)
        {
            TV value;
            return this.TryGetValue(key, out value);
        }

        [Conditional("DEBUG")]
        public void AssertBalanced()
        {
#if DEBUG
            AvlNode.AssertBalanced(this.root);
#endif
        }

        private abstract class Node
        {
            public readonly TK Key;
            public TV Value;

            protected Node(TK key, TV value)
            {
                this.Key = key;
                this.Value = value;
            }

            public virtual Node Next => null;
        }

        private sealed class NodeLinked : Node
        {
            public NodeLinked(TK key, TV value, Node next)
                : base(key, value)
            {
                this.Next = next;
            }

            public override Node Next { get; }
        }

        private sealed class AvlNodeHead : AvlNode
        {
            public Node next;

            public AvlNodeHead(int hashCode, TK key, TV value, Node next)
                : base(hashCode, key, value)
            {
                this.next = next;
            }

            public override Node Next => this.next;
        }

        // separate class to ensure that HashCode field 
        // is located before other AvlNode fields
        // Balance is also here for better packing of AvlNode on 64bit
        private abstract class HashedNode : Node
        {
            public readonly int HashCode;
            public sbyte Balance;

            protected HashedNode(int hashCode, TK key, TV value)
                : base(key, value)
            {
                this.HashCode = hashCode;
            }
        }

        private class AvlNode : HashedNode
        {
            public AvlNode Left;
            public AvlNode Right;

            public AvlNode(int hashCode, TK key, TV value)
                : base(hashCode, key, value)
            { }

#if DEBUG
            public static int AssertBalanced(AvlNode v)
            {
                if (v == null) return 0;

                int a = AssertBalanced(v.Left);
                int b = AssertBalanced(v.Right);

                if (a - b != v.Balance ||
                    Math.Abs(a - b) >= 2)
                {
                    throw new InvalidOperationException();
                }

                return 1 + Math.Max(a, b);
            }

#endif
        }

        private bool TryGetValue(int hashCode, TK key, out TV value)
        {
            AvlNode b = this.root;

            do
            {
                if (b.HashCode > hashCode)
                {
                    b = b.Left;
                }
                else if (b.HashCode < hashCode)
                {
                    b = b.Right;
                }
                else
                {
                    goto hasBucket;
                }
            }
 while (b != null);

            value = default(TV);
            return false;

            hasBucket:
            if (this.CompareKeys(b.Key, key))
            {
                value = b.Value;
                return true;
            }

            return this.GetFromList(b.Next, key, out value);
        }

        private bool GetFromList(Node next, TK key, out TV value)
        {
            while (next != null)
            {
                if (this.CompareKeys(key, next.Key))
                {
                    value = next.Value;
                    return true;
                }

                next = next.Next;
            }

            value = default(TV);
            return false;
        }

        private void Insert(int hashCode, TK key, TV value, bool add)
        {
            AvlNode currentNode = this.root;

            if (currentNode == null)
            {
                this.root = new AvlNode(hashCode, key, value);
                return;
            }

            AvlNode currentNodeParent = null;
            AvlNode unbalanced = currentNode;
            AvlNode unbalancedParent = null;

            // ====== insert new node
            // also make a note of the last unbalanced node and its parent (for rotation if needed)
            // nodes on the search path from rotation candidate downwards will change balances because of the node added
            // unbalanced node itself will become balanced or will be rotated
            // either way nodes above unbalanced do not change their balance
            for (;;)
            {
                // schedule hk read 
                var hc = currentNode.HashCode;

                if (currentNode.Balance != 0)
                {
                    unbalancedParent = currentNodeParent;
                    unbalanced = currentNode;
                }

                if (hc > hashCode)
                {
                    if (currentNode.Left == null)
                    {
                        currentNode.Left = currentNode = new AvlNode(hashCode, key, value);
                        break;
                    }

                    currentNodeParent = currentNode;
                    currentNode = currentNode.Left;
                }
                else if (hc < hashCode)
                {
                    if (currentNode.Right == null)
                    {
                        currentNode.Right = currentNode = new AvlNode(hashCode, key, value);
                        break;
                    }

                    currentNodeParent = currentNode;
                    currentNode = currentNode.Right;
                }
                else
                {
                    // (p.HashCode == hashCode)
                    this.HandleInsert(currentNode, currentNodeParent, key, value, add);
                    return;
                }
            }

            Debug.Assert(unbalanced != currentNode);

            // ====== update balances on the path from unbalanced downwards
            var n = unbalanced;
            do
            {
                Debug.Assert(n.HashCode != hashCode);

                if (n.HashCode < hashCode)
                {
                    n.Balance--;
                    n = n.Right;
                }
                else
                {
                    n.Balance++;
                    n = n.Left;
                }
            }
            while (n != currentNode);

            // ====== rotate unbalanced node if needed
            AvlNode rotated;
            var balance = unbalanced.Balance;
            if (balance == -2)
            {
                rotated = unbalanced.Right.Balance < 0 ?
                    LeftSimple(unbalanced) :
                    LeftComplex(unbalanced);
            }
            else if (balance == 2)
            {
                rotated = unbalanced.Left.Balance > 0 ?
                    RightSimple(unbalanced) :
                    RightComplex(unbalanced);
            }
            else
            {
                return;
            }

            // ===== make parent to point to rotated
            if (unbalancedParent == null)
            {
                this.root = rotated;
            }
            else if (unbalanced == unbalancedParent.Left)
            {
                unbalancedParent.Left = rotated;
            }
            else
            {
                unbalancedParent.Right = rotated;
            }
        }

        private static AvlNode LeftSimple(AvlNode unbalanced)
        {
            var right = unbalanced.Right;
            unbalanced.Right = right.Left;
            right.Left = unbalanced;

            unbalanced.Balance = 0;
            right.Balance = 0;
            return right;
        }

        private static AvlNode RightSimple(AvlNode unbalanced)
        {
            var left = unbalanced.Left;
            unbalanced.Left = left.Right;
            left.Right = unbalanced;

            unbalanced.Balance = 0;
            left.Balance = 0;
            return left;
        }

        private static AvlNode LeftComplex(AvlNode unbalanced)
        {
            var right = unbalanced.Right;
            var rightLeft = right.Left;
            right.Left = rightLeft.Right;
            rightLeft.Right = right;
            unbalanced.Right = rightLeft.Left;
            rightLeft.Left = unbalanced;

            var rightLeftBalance = rightLeft.Balance;
            rightLeft.Balance = 0;

            if (rightLeftBalance < 0)
            {
                right.Balance = 0;
                unbalanced.Balance = 1;
            }
            else
            {
                right.Balance = (sbyte)-rightLeftBalance;
                unbalanced.Balance = 0;
            }

            return rightLeft;
        }

        private static AvlNode RightComplex(AvlNode unbalanced)
        {
            var left = unbalanced.Left;
            var leftRight = left.Right;
            left.Right = leftRight.Left;
            leftRight.Left = left;
            unbalanced.Left = leftRight.Right;
            leftRight.Right = unbalanced;

            var leftRightBalance = leftRight.Balance;
            leftRight.Balance = 0;

            if (leftRightBalance < 0)
            {
                left.Balance = 1;
                unbalanced.Balance = 0;
            }
            else
            {
                left.Balance = 0;
                unbalanced.Balance = (sbyte)-leftRightBalance;
            }

            return leftRight;
        }


        private void HandleInsert(AvlNode node, AvlNode parent, TK key, TV value, bool add)
        {
            Node currentNode = node;
            do
            {
                if (this.CompareKeys(currentNode.Key, key))
                {
                    if (add)
                    {
                        throw new InvalidOperationException();
                    }

                    currentNode.Value = value;
                    return;
                }

                currentNode = currentNode.Next;
            }
 while (currentNode != null);

            this.AddNode(node, parent, key, value);
        }

        private void AddNode(AvlNode node, AvlNode parent, TK key, TV value)
        {
            AvlNodeHead head = node as AvlNodeHead;
            if (head != null)
            {
                var newNext = new NodeLinked(key, value, head.next);
                head.next = newNext;
                return;
            }

            var newHead = new AvlNodeHead(node.HashCode, key, value, node);
            newHead.Balance = node.Balance;
            newHead.Left = node.Left;
            newHead.Right = node.Right;

            if (parent == null)
            {
                this.root = newHead;
                return;
            }

            if (node == parent.Left)
            {
                parent.Left = newHead;
            }
            else
            {
                parent.Right = newHead;
            }
        }

        public KeyCollection Keys => new KeyCollection(this);

        public struct KeyCollection : IEnumerable<TK>
        {
            private readonly SmallDictionary<TK, TV> dict;

            public KeyCollection(SmallDictionary<TK, TV> dict)
            {
                this.dict = dict;
            }

            public struct Enumerator
            {
                private readonly Stack<AvlNode> stack;
                private Node next;
                private Node current;

                public Enumerator(SmallDictionary<TK, TV> dict)
                    : this()
                {
                    var root = dict.root;
                    if (root != null)
                    {
                        // left == right only if both are nulls
                        if (root.Left == root.Right)
                        {
                            this.next = dict.root;
                        }
                        else
                        {
                            this.stack = new Stack<AvlNode>(dict.HeightApprox());
                            this.stack.Push(dict.root);
                        }
                    }
                }

                public TK Current => this.current.Key;

                public bool MoveNext()
                {
                    if (this.next != null)
                    {
                        this.current = this.next;
                        this.next = this.next.Next;
                        return true;
                    }

                    if (this.stack == null || this.stack.Count == 0)
                    {
                        return false;
                    }

                    var curr = this.stack.Pop();
                    this.current = curr;
                    this.next = curr.Next;

                    this.PushIfNotNull(curr.Left);
                    this.PushIfNotNull(curr.Right);

                    return true;
                }

                private void PushIfNotNull(AvlNode child)
                {
                    if (child != null)
                    {
                        this.stack.Push(child);
                    }
                }
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(this.dict);
            }

            public class EnumerableImpl : IEnumerator<TK>
            {
                private Enumerator e;

                public EnumerableImpl(Enumerator e)
                {
                    this.e = e;
                }

                TK IEnumerator<TK>.Current => this.e.Current;

                void IDisposable.Dispose()
                {
                }

                object IEnumerator.Current => this.e.Current;

                bool IEnumerator.MoveNext()
                {
                    return this.e.MoveNext();
                }

                void IEnumerator.Reset()
                {
                    throw new NotSupportedException();
                }
            }

            IEnumerator<TK> IEnumerable<TK>.GetEnumerator()
            {
                return new EnumerableImpl(this.GetEnumerator());
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        public ValueCollection Values => new ValueCollection(this);

        ICollection<TK> IDictionary<TK, TV>.Keys
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        ICollection<TV> IDictionary<TK, TV>.Values
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public struct ValueCollection : IEnumerable<TV>
        {
            private readonly SmallDictionary<TK, TV> dict;

            public ValueCollection(SmallDictionary<TK, TV> dict)
            {
                this.dict = dict;
            }

            public struct Enumerator
            {
                private readonly Stack<AvlNode> stack;
                private Node next;
                private Node current;

                public Enumerator(SmallDictionary<TK, TV> dict)
                    : this()
                {
                    var root = dict.root;
                    if (root == null)
                    {
                        return;
                    }

                    // left == right only if both are nulls
                    if (root.Left == root.Right)
                    {
                        this.next = dict.root;
                    }
                    else
                    {
                        this.stack = new Stack<AvlNode>(dict.HeightApprox());
                        this.stack.Push(dict.root);
                    }
                }

                public TV Current => this.current.Value;

                public bool MoveNext()
                {
                    if (this.next != null)
                    {
                        this.current = this.next;
                        this.next = this.next.Next;
                        return true;
                    }

                    if (this.stack == null || this.stack.Count == 0)
                    {
                        return false;
                    }

                    var curr = this.stack.Pop();
                    this.current = curr;
                    this.next = curr.Next;

                    this.PushIfNotNull(curr.Left);
                    this.PushIfNotNull(curr.Right);

                    return true;
                }

                private void PushIfNotNull(AvlNode child)
                {
                    if (child != null)
                    {
                        this.stack.Push(child);
                    }
                }
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(this.dict);
            }

            public class EnumerableImpl : IEnumerator<TV>
            {
                private Enumerator e;

                public EnumerableImpl(Enumerator e)
                {
                    this.e = e;
                }

                TV IEnumerator<TV>.Current => this.e.Current;

                void IDisposable.Dispose()
                {
                }

                object IEnumerator.Current => this.e.Current;

                bool IEnumerator.MoveNext()
                {
                    return this.e.MoveNext();
                }

                void IEnumerator.Reset()
                {
                    throw new NotImplementedException();
                }
            }

            IEnumerator<TV> IEnumerable<TV>.GetEnumerator()
            {
                return new EnumerableImpl(this.GetEnumerator());
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        public struct Enumerator
        {
            private readonly Stack<AvlNode> stack;
            private Node next;
            private Node current;

            public Enumerator(SmallDictionary<TK, TV> dict)
                : this()
            {
                var root = dict.root;
                if (root == null)
                {
                    return;
                }

                // left == right only if both are nulls
                if (root.Left == root.Right)
                {
                    this.next = dict.root;
                }
                else
                {
                    this.stack = new Stack<AvlNode>(dict.HeightApprox());
                    this.stack.Push(dict.root);
                }
            }

            public KeyValuePair<TK, TV> Current => new KeyValuePair<TK, TV>(this.current.Key, this.current.Value);

            public bool MoveNext()
            {
                if (this.next != null)
                {
                    this.current = this.next;
                    this.next = this.next.Next;
                    return true;
                }

                if (this.stack == null || this.stack.Count == 0)
                {
                    return false;
                }

                var curr = this.stack.Pop();
                this.current = curr;
                this.next = curr.Next;

                this.PushIfNotNull(curr.Left);
                this.PushIfNotNull(curr.Right);

                return true;
            }

            private void PushIfNotNull(AvlNode child)
            {
                if (child != null)
                {
                    this.stack.Push(child);
                }
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public class EnumerableImpl : IEnumerator<KeyValuePair<TK, TV>>
        {
            private Enumerator e;

            public EnumerableImpl(Enumerator e)
            {
                this.e = e;
            }

            KeyValuePair<TK, TV> IEnumerator<KeyValuePair<TK, TV>>.Current => this.e.Current;

            void IDisposable.Dispose()
            {
            }

            object IEnumerator.Current => this.e.Current;

            bool IEnumerator.MoveNext()
            {
                return this.e.MoveNext();
            }

            void IEnumerator.Reset()
            {
                throw new NotImplementedException();
            }
        }

        IEnumerator<KeyValuePair<TK, TV>> IEnumerable<KeyValuePair<TK, TV>>.GetEnumerator()
        {
            return new EnumerableImpl(this.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        private int HeightApprox()
        {
            // height is less than 1.5 * depth(leftmost node)
            var h = 0;
            var cur = this.root;
            while (cur != null)
            {
                h++;
                cur = cur.Left;
            }

            h = h + h / 2;
            return h;
        }

        public bool Remove(TK key)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<TK, TV> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            this.root = null;
        }

        public bool Contains(KeyValuePair<TK, TV> item)
        {
            return this.ContainsKey(item.Key) && this[item.Key].Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<TK, TV>[] array, int arrayIndex)
        {
            var list = this.ToList();
            for (int i = arrayIndex; i < this.Count; i++)
                array[i] = list[i - arrayIndex];
        }

        public bool Remove(KeyValuePair<TK, TV> item)
        {
            return this.Remove(item.Key);
        }
    }
}
