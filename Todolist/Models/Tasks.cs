using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Todolist.Models
{
    [Table("tblTask")]
    public class Tasks
    {
        [Key]
        public int TaskID { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set;}
        public string TagName { get; set;}
        [ForeignKey("TagID")]
        public int? TagID { get; set; }
        [ForeignKey("UserID")]
        public int? UserID { get; set; }
    }
}
