namespace MiniCRM.Models
{
    public class ContactsIndexViewModel
    {
        public string? Search { get; set; }
        public string? Company { get; set; }
        public string? SortOrder { get; set; } // "lname_asc","lname_desc","company_asc","company_desc"

        public PagedResult<Contact> Page { get; set; } = new PagedResult<Contact>();
    }
}
