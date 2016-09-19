using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace UstbBox.Wpf.Models
{
    public class CredentialKind
    {
        public CredentialKind(string id, string name, string defaultPasswordInformation, params string[] websites)
        {
            this.Id = id;
            this.Name = name;
            this.DefaultPasswordInfomation = defaultPasswordInformation;
            this.Websites = new ReadOnlyCollection<string>(websites);
        }

        public string Id { get; }

        public string Name { get; }

        public string DefaultPasswordInfomation { get; }

        public ReadOnlyCollection<string> Websites { get; set; }

        public static CredentialKind CourseCenter { get; } = new CredentialKind("71f7108d-3321-4a82-87a1-c0cbba6f6f5f", "课程中心", "默认密码可能为学号", "cc.ustb.edu.cn");

        public static CredentialKind Common { get; } = new CredentialKind("c2cf760f-dd19-47b3-825a-dcfbf0988e6c", "统一认证平台", "", "e.ustb.edu.cn", "zhiyuan.ustb.edu.cn");

        public static CredentialKind EduSystem { get; } = new CredentialKind("e5c07722-0f01-49e5-be9a-b8651764fbd5", "教务管理系统", "", "elearning.ustb.edu.cn");

        public static CredentialKind Network { get; } = new CredentialKind("7e63931c-0479-4b33-8227-2763d738a437", "校园网", "", "login.ustb.edu.cn", "202.204.60.7:8080");

        public static CredentialKind Library { get; } = new CredentialKind("dd48f5a3-4561-4374-b14e-942df2b27289", "图书馆", "2013年9月以前入校的：密码为读者证件号或者校园卡卡号后六位\n2013年10月以后入校的：密码为六个零", "lib.ustb.edu.cn");

        public static IReadOnlyCollection<CredentialKind> AllKinds { get; } = new List<CredentialKind>() { CourseCenter, Common, EduSystem, Network, Library };

        public static IReadOnlyCollection<string> AllKindIds { get; } = AllKinds.Select(x => x.Id).ToList();

        public static CredentialKind GetKindById(string id)
        {
            return AllKinds.First(x => x.Id == id);
        }
    }
}
