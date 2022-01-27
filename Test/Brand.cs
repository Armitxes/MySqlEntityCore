namespace MySqlEntityCore {

    [Model]
    class Brand : Template.DefaultModel
    {
        [Field(Size=45)]
        public string Name { get; set; }

        [Field(Size=45)]
        public string Key { get; set; }  // Key = MySql Keyword
    }

}
