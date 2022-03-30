namespace MySqlEntityCore.Test
{
    [Model]
    public class GroupUser : MySqlEntityCore.Template.Core
    {
        [Field(PrimaryKey = true)]
        public Group Group { get; set; }

        [Field(PrimaryKey = true)]
        public User User { get; set; }
    }
}
