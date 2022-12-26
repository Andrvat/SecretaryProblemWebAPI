using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataContracts.Entities;

public class RatingContenderEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ContenderId { get; set;  }
    
    public string Surname { get; set; }
    
    public string Name { get; set; }

    public int Rating { get; set; }
    
    
}