namespace MySqlEntityCore.Test
{
    [Model]
    public class User : MySqlEntityCore.Template.DefaultModel
    {

        [Field(Size = 45, Unique = true, Required = true)]
        public string Username { get; set; }

        [Field(Size = 64)]
        public string Password { get; set; }

        [Field]
        public bool Employee { get; set; }

    }
}
