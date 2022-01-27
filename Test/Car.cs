namespace MySqlEntityCore {

    [Model]
    class Car : Template.DefaultModel
    {
        [Field(Size=45)]
        public string Name { get; set; }

        [Field]
        public Brand Brand { get; set; }
    }

}
