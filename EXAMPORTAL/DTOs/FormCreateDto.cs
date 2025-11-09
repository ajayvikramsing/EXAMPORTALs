namespace EXAMPORTAL.DTOs
{
    public class FormCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string FieldsJson { get; set; } // front-end will send valid JSON describing fields
        public bool IsActive { get; set; } = true;
    }

    public class FormListDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } // hiding  the proeprties from client 
    }
}
