using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Coin.Models
{

public class UserProfile{
     public int id { get; set; }
     public int balance { get; set; }
     public string name{get;set;}

     public string crew{get;set;}

     public string type{get;set;}

     public string phonenum{get;set;}

     public Boolean admin{get;set;}
}

}
