using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Coin.Models;
using MySql.Data.MySqlClient;
using Microsoft.Data.SqlClient;

namespace Coin.Controllers;
[Route("api/[Controller]")]
[ApiController]
public class CoinController :ControllerBase{


//  MySqlConnectionStringBuilder _context = new MySqlConnectionStringBuilder{
//         Server = "memo.cxywos9kigxi.ap-northeast-2.rds.amazonaws.com",
//         Database="MAP",
//         UserID="admin",
//         Password="memomemo!",
//         Port=3306,
//         ConnectionTimeout =60,
//         AllowZeroDateTime=true
//  };
public MySqlConnection connection = new MySqlConnection(new MySqlConnectionStringBuilder{
        Server = "memo.cxywos9kigxi.ap-northeast-2.rds.amazonaws.com",
        Database="MAP",
        UserID="admin",
        Password="memomemo!",
        Port=3306,
        ConnectionTimeout =60,
        AllowZeroDateTime=true
 }.ConnectionString);

[HttpPost("login")]
public IActionResult login( [FromBody]login param){

using(connection){
            connection.Open();
             MySqlCommand cmd = new(@"
                select * from 
                Users 
                WHERE phonenum = @phonenum
            ", connection);
            cmd.Parameters.AddWithValue("@phonenum",param.password);
            cmd.Parameters["@phonenum"].Value=param.password;
            var profile =new UserProfile();
using (MySqlDataReader reader = cmd.ExecuteReader()){
             while(reader.Read()){
                    profile=new UserProfile{
                        id = Convert.ToInt32(reader["id"]),
                    name = reader["name"].ToString(),
                    crew = reader["crew"].ToString(),
                    type = reader["type"].ToString(),
                    balance = Convert.ToInt32(reader["balance"]),
                    phonenum = reader["phonenum"].ToString(),
                    admin = Convert.ToBoolean(reader["admin"])
                };
        
   
        }
         return Ok(profile);
    }
}
}
//  while(reader.Read()){
            //         profile.Add(new UserProfile{
            //             id = Convert.ToInt32(reader["user_id"]),
            //         userId = reader["user_name"].ToString(),
            //         crew = reader["crew"].ToString(),
            //         typo = reader["typo"].ToString()
            //     });
            // }
// [HttpGet("holdings/{id}")]
// public IActionResult GetHoldings(int id){
// using(connection){
//             connection.Open();
//              MySqlCommand cmd = new(@"
//                 SELECT
//                     *
//                 FROM wallet
//                 WHERE user_id = @id
//             ", connection);
//             cmd.Parameters.AddWithValue("@id",id);
//             cmd.Parameters["@id"].Value=id;
//             var Wallet = new List<Wallet>();
// using (MySqlDataReader reader = cmd.ExecuteReader()){
//             while(reader.Read()){
//                     Wallet.Add(new Wallet{
//                 wallet = Convert.ToInt32(reader["wallet_id"]),
//                 wap = Convert.ToInt32(reader["wap"]),
//                 app = Convert.ToInt32(reader["app"]),
//                 mut = Convert.ToInt32(reader["mut"]),
//                 pknu = Convert.ToInt32(reader["pknu"]),
//                 pus = Convert.ToInt32(reader["pus"]),
//                 pufs = Convert.ToInt32(reader["pufs"]),
//                 total = Convert.ToInt32(reader["balance"])
//                 });
//             }
//     return Ok(Wallet);
//         }
//     }



// }
// [HttpGet("coin")]
// public IActionResult GetCoin(){
// return Ok();
// }
// [HttpGet("ranking")]



}