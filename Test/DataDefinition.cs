namespace MySqlEntityCore.Test
{
    [Model]
    public class Group : MySqlEntityCore.Template.DefaultModel
    {

        [Field(Size = 45, Required = true)]
        public string Name { get; set; }

        [Field(Size = 45, Unique = true, Required = true)]
        public string Key { get; set; }

    }

    // Test Relations
    [Model]
    public class GroupUser : MySqlEntityCore.Template.Core
    {
        [Field(PrimaryKey = true)]
        public Group Group { get; set; }

        [Field(PrimaryKey = true)]
        public User User { get; set; }
    }
}
