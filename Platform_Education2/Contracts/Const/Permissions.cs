namespace PlatformEduPro.Contracts.Const
{
    public static class Permissions
    {
        public static string Type { get; } = "permissions";



        public const string Course = "Course";
        public const string Course_Create = "Course.Create";
        public const string Course_Update = "Course.Update";
        public const string Course_Delete = "Course.Delete";
        public const string Course_GetAll = "Course.GetAll";
        public const string Course_GetById = "Course.GetById";


        public const string Question_Create = " Question.Create";
        public const string Question_Update = "Question.Update";
        public const string Question_Delete = "Question.Delete";
        public const string Question_GetAll = "Question.GetAll";
        public const string Question_GetById = "Question.GetById";


        public const string Section_Create = " Section.Create";
        public const string Section_Update = "Section.Update";
        public const string Section_Delete = "Section.Delete";
        public const string Section_GetAll = "Section.GetAll";
        public const string Section_GetById = "Section.GetById";



        public const string Video_Create = " Video.Create";
        public const string Video_Update = " Video.Update";
        public const string Video_Delete = " Video.Delete";
        public const string Video_GetAll = " Video.GetAll";
        public const string Video_GetById = "Video.GetById";





        public const string GetUsers = "users:read";
        public const string AddUsers = "users:add";
        public const string UpdateUsers = "users:update";
        public const string ToggleStatus = "users:Toggle";
        public const string UnlockUsers = "users:Unlock";

        public const string GetRoles = "roles:read";
        public const string AddRoles = "roles:add";
        public const string UpdateRoles = "roles:update";

    
         
        public static IList<string?> GetAllPermissions() =>
            typeof(Permissions).GetFields().Select(x => x.GetValue(x) as string).ToList();


    }
}
