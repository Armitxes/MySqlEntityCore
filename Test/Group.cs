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
}
