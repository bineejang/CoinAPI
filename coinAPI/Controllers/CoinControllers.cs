using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Coin.Models;
using MySql.Data.MySqlClient;
using Microsoft.Data.SqlClient;

namespace Coin.Controllers;
[Route("api/[Controller]")]
[ApiController]
public class CoinController :ControllerBase{

 MySqlConnectionStringBuilder _context = new MySqlConnectionStringBuilder{
        Server = "memo.cxywos9kigxi.ap-northeast-2.rds.amazonaws.com",
        Database="MAP",
        UserID="admin",
        Password="memomemo!",
        Port=3306,
        ConnectionTimeout =60,
        AllowZeroDateTime=true
 };

[HttpGet("my-profile/{id}")]
public IActionResult Test( int id){

using(MySqlConnection connection = new MySqlConnection(_context.ConnectionString)){
            connection.Open();
             MySqlCommand cmd = new(@"
                SELECT
                    *
                FROM user
                WHERE user_id = @id
            ", connection);
            cmd.Parameters.AddWithValue("@id",id);
            cmd.Parameters["@id"].Value=id;
            var profile = new List<UserProfile>();
using (MySqlDataReader reader = cmd.ExecuteReader()){
            while(reader.Read()){
                    profile.Add(new UserProfile{
                        id = Convert.ToInt32(reader["user_id"]),
                    userId = reader["user_name"].ToString(),
                    crew = reader["crew"].ToString(),
                    typo = reader["typo"].ToString()
                });
            }


    return Ok(profile);
        }
    }
}
[HttpGet("holdings/{id}")]
public IActionResult GetHoldings(int id){




using(MySqlConnection connection = new MySqlConnection(_context.ConnectionString)){
            connection.Open();
             MySqlCommand cmd = new(@"
                SELECT
                    *
                FROM wallet
                WHERE user_id = @id
            ", connection);
            cmd.Parameters.AddWithValue("@id",id);
            cmd.Parameters["@id"].Value=id;
            var Wallet = new List<Wallet>();
using (MySqlDataReader reader = cmd.ExecuteReader()){
            while(reader.Read()){
                    Wallet.Add(new Wallet{
                wallet = Convert.ToInt32(reader["wallet_id"]),
                wap = Convert.ToInt32(reader["wap"]),
                app = Convert.ToInt32(reader["app"]),
                mut = Convert.ToInt32(reader["mut"]),
                pknu = Convert.ToInt32(reader["pknu"]),
                pus = Convert.ToInt32(reader["pus"]),
                pufs = Convert.ToInt32(reader["pufs"]),
                total = Convert.ToInt32(reader["balance"])
                });
            }
    return Ok(Wallet);
        }
    }



}
}