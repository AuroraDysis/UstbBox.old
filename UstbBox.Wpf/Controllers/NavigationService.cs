namespace UstbBox.Wpf.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Reactive.Bindings;
    using UstbBox.Wpf.Navigation;

    public class NavigationService
    {
        private readonly LinkedList<NavigationJournal> journals = new LinkedList<NavigationJournal>();
        private readonly HashSet<CacheItem> navigationCache = new HashSet<CacheItem>();
        private LinkedListNode<NavigationJournal> currentJournalNode;

        private NavigationService()
        {
            CommandGoBack = this.CanGoBack.ToReactiveCommand();
            CommandGoBack.Subscribe(_ => this.GoBack());

            CommandGoForward = this.CanGoForward.ToReactiveCommand();
            CommandGoForward.Subscribe(_ => this.GoForward());

            CommandNavigateTo = new ReactiveCommand<Type>();
            CommandNavigateTo.Subscribe(type => this.NavigateTo(type));

            CommandNavigateToItem = new ReactiveCommand<NavigationItem>();
            CommandNavigateToItem.Subscribe(item => this.NavigateTo(item.ViewModelType, item.Parameter));
        }

        public static NavigationService Instance { get; } = new NavigationService();

        public static ReactiveCommand CommandGoBack { get; private set; }

        public static ReactiveCommand CommandGoForward { get; private set; }

        public static ReactiveCommand<Type> CommandNavigateTo { get; private set; }

        public static ReactiveCommand<NavigationItem> CommandNavigateToItem { get; private set; }

        public ReactiveCollection<WeakReference> SlidersSource { get; set; } = new ReactiveCollection<WeakReference>();

        public ReactiveProperty<int> ActiveSliderIndex { get; set; } = new ReactiveProperty<int>(-1);

        public ReactiveProperty<bool> CanGoForward { get; set; } = new ReactiveProperty<bool>();

        public ReactiveProperty<bool> CanGoBack { get; set; } = new ReactiveProperty<bool>();

        public void NavigateTo<TViewModel>()
        {
            this.NavigateTo(typeof(TViewModel));
        }

        public void NavigateTo<TViewModel>(object parameter)
        {
            this.NavigateTo(typeof(TViewModel), parameter);
        }

        public void NavigateTo(Type viewModelType)
        {
            this.NavigateTo(viewModelType, null);
        }

        public void NavigateTo(Type viewModelType, object parameter)
        {
            var journal = new NavigationJournal(this.ActiveSliderIndex.Value + 1, viewModelType, parameter);
            var newNode = new LinkedListNode<NavigationJournal>(journal);
            if (this.currentJournalNode == null)
            {
                this.journals.AddLast(newNode);
            }
            else
            {
                this.journals.AddAfter(this.currentJournalNode, newNode);
                var tail = newNode.Next;
                while (tail != null)
                {
                    this.journals.Remove(tail);
                    tail = tail.Next;
                }
            }

            this.currentJournalNode = newNode;
            this.NavigateToCurrentNode();
        }

        public void GoBack()
        {
            if (this.currentJournalNode.Previous != null)
            {
                this.currentJournalNode = this.currentJournalNode.Previous;
                this.NavigateToCurrentNode();
            }
        }

        public void GoForward()
        {
            if (this.currentJournalNode.Next != null)
            {
                this.currentJournalNode = this.currentJournalNode.Next;
                this.NavigateToCurrentNode();
            }
        }

        private void NavigateToCurrentNode()
        {
            var current = this.currentJournalNode.Value;
            var cacheItem = this.navigationCache.FirstOrDefault(this.FindCache);
            if (cacheItem != null)
            {
                cacheItem.Access();
                cacheItem.NavigationItem.OnNavigatedTo(current.Parameter);
                this.NavigateToAwareItem(cacheItem.NavigationItem);
            }
            else
            {
                var item = (INavigationAware)Activator.CreateInstance(current.ViewModelType);
                var redirect = item.Redirect(current.Parameter);
                if (redirect)
                {
                    item.Dispose();
                }
                else
                {
                    this.navigationCache.Add(new CacheItem(item, current.Parameter));
                    item.Initialize(current.Parameter);
                    item.OnNavigatedTo(current.Parameter);
                    this.NavigateToAwareItem(item);
                    this.ClearMoreCache();
                }
            }
        }

        private void NavigateToAwareItem(INavigationAware item)
        {
            var current = this.currentJournalNode.Value;
            this.CanGoBack.Value = this.currentJournalNode.Previous != null;
            this.CanGoForward.Value = this.currentJournalNode.Next != null;
            var index = current.Index;
            if (this.ActiveSliderIndex.Value != -1)
            {
                var old = this.SlidersSource[this.ActiveSliderIndex.Value];
                if (old.IsAlive)
                {
                    ((INavigationAware)old.Target).OnNavigatedFrom();
                }
            }

            if (index == this.SlidersSource.Count)
            {
                this.SlidersSource.Add(new WeakReference(item));
                this.ActiveSliderIndex.Value = this.SlidersSource.Count - 1;
            }
            else
            {
                this.SlidersSource.SetOnScheduler(index, new WeakReference(item));
                this.ActiveSliderIndex.Value = index;
            }
        }

        private void ClearMoreCache()
        {
            if (this.navigationCache.Count > 3)
            {
                foreach (var item in this.navigationCache.OrderByDescending(x => x.LastAccessTime).Skip(3))
                {
                    this.navigationCache.Remove(item);
                    item.NavigationItem.Dispose();
                }
            }
        }

        private bool FindCache(CacheItem item)
        {
            return item.NavigationItem.GetType() == this.currentJournalNode.Value.ViewModelType && item.Parameter == this.currentJournalNode.Value.Parameter;
        }

        private class NavigationJournal
        {
            public NavigationJournal(int index, Type viewModelType, object parameter)
            {
                this.Index = index;
                this.ViewModelType = viewModelType;
                this.Parameter = parameter;
            }

            public int Index { get; }

            public Type ViewModelType { get; }

            public object Parameter { get; }
        }

        private class CacheItem
        {
            public CacheItem(INavigationAware item, object parameter)
            {
                this.NavigationItem = item;
                this.Parameter = parameter;
            }

            public DateTime LastAccessTime { get; private set; }

            public INavigationAware NavigationItem { get; }

            public object Parameter { get; }

            public void Access()
            {
                this.LastAccessTime = DateTime.Now;
            }
        }
    }
}
