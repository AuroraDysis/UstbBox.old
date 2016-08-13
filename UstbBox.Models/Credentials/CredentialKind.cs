using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Models.Credentials
{
    using System.Collections.ObjectModel;

    public class CredentialKind
    {
        public static CredentialKind Get(Guid id) => idMapper[id.ToString()];

        public static CredentialKind Get(string id) => idMapper[id];

        private static readonly Dictionary<string, CredentialKind> idMapper = new Dictionary<string, CredentialKind>();

        public static IReadOnlyCollection<CredentialKind> AllKinds
            => new List<CredentialKind> { CourseCenter, Network, EducationSystem, Common, Library };

        public static CredentialKind CourseCenter { get; set; } =
            new CredentialKind("71f7108d-3321-4a82-87a1-c0cbba6f6f5f", "课程中心", "默认密码可能为学号", "cc.ustb.edu.cn");

        public static CredentialKind Network { get; set; } = new CredentialKind(
            "7e63931c-0479-4b33-8227-2763d738a437",
            "校园网",
            "",
            "login.ustb.edu.cn",
            "202.204.60.7:8080");

        public static CredentialKind EducationSystem { get; set; } = new CredentialKind(
            "e5c07722-0f01-49e5-be9a-b8651764fbd5",
            "教务管理系统",
            "",
            "elearning.ustb.edu.cn");

        public static CredentialKind Common { get; set; } = new CredentialKind(
            "c2cf760f-dd19-47b3-825a-dcfbf0988e6c",
            "统一认证平台",
            "",
            "e.ustb.edu.cn",
            "zhiyuan.ustb.edu.cn");

        public static CredentialKind Library { get; set; } = new CredentialKind(
            "dd48f5a3-4561-4374-b14e-942df2b27289",
            "图书馆",
            "2013年9月以前入校的：密码为读者证件号或者校园卡卡号后六位\n2013年10月以后入校的：密码为六个零",
            "lib.ustb.edu.cn");

        private CredentialKind(string id, string name, string defaultPasswordInfomation, params string[] websites)
        {
            this.Id = Guid.Parse(id);
            this.Name = name;
            this.DefaultPasswordInfomation = defaultPasswordInfomation;
            this.Websites = new ReadOnlyCollection<string>(websites);
            idMapper.Add(this.Id.ToString(), this);
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string DefaultPasswordInfomation { get; set; }

        public IReadOnlyCollection<string> Websites { get; set; }
    }
}
