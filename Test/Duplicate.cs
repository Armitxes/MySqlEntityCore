// Should be able to handle but give warning
// Also test behaviour of existing FK constrain 

namespace MySqlEntityCore.Test
{
    [Model]
    public class Duplicate : MySqlEntityCore.Template.DefaultModel
    {

        [Field(Required = true)]
        public string NameA { get; set; }

        [Field]
        public Duplicate Parent { get; set; }

    }
}

namespace MySqlEntityCore.Test.Conflict
{
    [Model]
    public class Duplicate : MySqlEntityCore.Template.DefaultModel
    {

        [Field(Required = true)]
        public string NameB { get; set; }

        [Field]
        public Duplicate Parent { get; set; }

    }
}
