using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Todolist.Models
{
    [Table("tblTag")]
    public class Tags
    {
        [Key]
        public int TagID { get; set; }
        public string TagName { get; set; }
    }
}
