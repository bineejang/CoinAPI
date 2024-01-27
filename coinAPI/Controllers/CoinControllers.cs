using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Coin.Models;
using MySql.Data.MySqlClient;
using Microsoft.Data.SqlClient;
using Org.BouncyCastle.Asn1.Cms;
using Microsoft.EntityFrameworkCore.Query;
using System.Data;

namespace Coin.Controllers;
[Route("api/[Controller]")]
[ApiController]
public class CoinController : ControllerBase
{
    //  MySqlConnectionStringBuilder _context = new MySqlConnectionStringBuilder{
    //         Server = "memo.cxywos9kigxi.ap-northeast-2.rds.amazonaws.com",
    //         Database="MAP",
    //         UserID="admin",
    //         Password="memomemo!",
    //         Port=3306,
    //         ConnectionTimeout =60,
    //         AllowZeroDateTime=true
    //  };
    public MySqlConnection connection = new MySqlConnection(new MySqlConnectionStringBuilder
    {
        Server = "memo.cxywos9kigxi.ap-northeast-2.rds.amazonaws.com",
        Database = "MAP",
        UserID = "admin",
        Password = "memomemo!",
        Port = 3306,
        ConnectionTimeout = 60,
        AllowZeroDateTime = true
    }.ConnectionString);

    [HttpPost("login")]
    public IActionResult login([FromBody] login param)
    {

        using (connection)
        {
            connection.Open();
            MySqlCommand cmd = new(@"
                select * from 
                Users 
                WHERE phonenum = @phonenum
            ", connection);
            cmd.Parameters.AddWithValue("@phonenum", param.password);
            cmd.Parameters["@phonenum"].Value = param.password;
            var profile = new UserProfile();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    profile = new UserProfile
                    {
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

    [HttpGet("holdings/{id}")]
    public IActionResult GetHoldings(int id)
    {
        using (connection)
        {
            connection.Open();
            MySqlCommand cmd = new(@"
                select coins.count from MAP.Users users
                join MAP.Coins coins on users.id = coins.userId
                join MAP.Coin coin on coins.coinId = coin.id
                where users.id = @id ORDER BY coins.coinId ASC;
            ", connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters["@id"].Value = id;
            var Wallet = new List<Wallet>();
            var list = new List<int>();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {

                for (int i = 0; i < 6; i++)
                {
                    reader.Read();

                    list.Add(Convert.ToInt32(reader["count"]));
                }
                Wallet.Add(new Wallet
                {
                    wap = list[0],
                    app = list[1],
                    mut = list[2],
                    pknu = list[3],
                    pus = list[4],
                    pufs = list[5],
                });


                return Ok(Wallet);
            }
        }
    }
    [HttpGet("coin")]
    public IActionResult getCoins()
    {
        using (connection)
        {
            connection.Open();
            MySqlCommand cmd = new(@"
            SELECT
                * 
            FROM 
                Coin
            ", connection);
            var Coin = new List<CoinPrice>();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Coin.Add(new CoinPrice
                    {
                        id = Convert.ToInt32(reader["id"]),
                        coinName = reader["coinName"].ToString(),
                        prevPrice = Convert.ToInt32(reader["prevPrice"]),
                        currentPrice = Convert.ToInt32(reader["currentPrice"]),
                        nextRate = Convert.ToInt32(reader["nextrate"]),
                    });

                }

            }
            return Ok(Coin);
        }
    }
    [HttpPost("coin/sell")]
    public IActionResult CoinSell([FromBody] Contract param)
    {
        using (connection)
        {
            connection.Open();
            int count = 0;
            int balance = 0;
            int currentPrice = 0;
            string coinName;
            MySqlCommand cmd = new(@"
            SELECT 
                count 
            FROM 
                Coins 
            WHERE 
                coinId = @coinId 
            AND 
                userId=@id
            ", connection);
            cmd.Parameters.AddWithValue("@coinId", param.coinId);
            cmd.Parameters.AddWithValue("@id", param.id);
            using (MySqlDataReader reader1 = cmd.ExecuteReader())
            {
                reader1.Read();
                count = Convert.ToInt16(reader1["count"]);
            }
            MySqlCommand setCountcmd = new(@"
            UPDATE 
                Coins 
            set 
                count = @count
            WHERE 
                coinId = @coinId 
            AND 
                userId=@id
            ", connection);
            setCountcmd.Parameters.AddWithValue("@coinId", param.coinId);
            setCountcmd.Parameters.AddWithValue("@id", param.id);
            setCountcmd.Parameters.AddWithValue("@count", count - param.count);
            setCountcmd.ExecuteNonQuery();
            MySqlCommand findcmd = new(@"
            SELECT
                coins.count, users.balance 
            FROM
                Coins coins
            JOIN 
                Users users 
            ON 
                coins.userId = users.id
            WHERE 
                userId = @userId 
            AND 
                coinId = @coinId
            ", connection);
            findcmd.Parameters.AddWithValue("@coinId", param.coinId);
            findcmd.Parameters.AddWithValue("@id", param.id);
            using (MySqlDataReader reader2 = cmd.ExecuteReader())
            {
                reader2.Read();
                count = Convert.ToInt16(reader2["count"]);
                balance = Convert.ToInt16(reader2["balance"]);
            }
            MySqlCommand pricecmd = new(@"
            SELECT 
                currentPrice 
            FROM     
                Coin 
            WHERE 
                id=@coinId
            ", connection);
            pricecmd.Parameters.AddWithValue("@coinId", param.coinId);
            using (MySqlDataReader reader3 = cmd.ExecuteReader())
            {
                reader3.Read();
                coinName = reader3["coinName"].ToString();
            }
            MySqlCommand calccmd = new(@"
            UPDATE 
                Users 
            SET
                balance = @balance
            UPDATE 
                Wallet 
            SET 
                @coinName = @coinBalance
            UPDATE 
                Wallet 
            SET 
                total =  P_WAP+P_MUT+P_APP+P_PKNU+P_PUS+P_PUFS
            ", connection);
            calccmd.Parameters.AddWithValue("@coinName", coinName);
            calccmd.Parameters.AddWithValue("@balance", balance + count * currentPrice);
            calccmd.Parameters.AddWithValue("@coinBalance", currentPrice * count);
            calccmd.ExecuteNonQuery();
            return Ok("매도체결");
        }

    }
    [HttpPost("coin/buy")]
    public IActionResult Coinbuy([FromBody] Contract param)
    {
        using (connection)
        {
            connection.Open();
            int count = 0;
            int balance = 0;
            int currentPrice = 0;
            string coinName;
            MySqlCommand cmd = new(@"
            SELECT 
                count 
            FROM 
                Coins 
            WHERE 
                coinId = @coinId 
            AND 
                userId=@id
            ", connection);
            cmd.Parameters.AddWithValue("@coinId", param.coinId);
            cmd.Parameters.AddWithValue("@id", param.id);
            using (MySqlDataReader reader1 = cmd.ExecuteReader())
            {
                reader1.Read();
                count = Convert.ToInt16(reader1["count"]);
            }
            MySqlCommand setCountcmd = new(@"
            UPDATE 
                coin 
            set 
                count = @count
            WHERE 
                coinId = @coinId 
            AND 
                userId=@id
            ", connection);
            setCountcmd.Parameters.AddWithValue("@coinId", param.coinId);
            setCountcmd.Parameters.AddWithValue("@id", param.id);
            setCountcmd.Parameters.AddWithValue("@count", count + param.count);
            setCountcmd.ExecuteNonQuery();
            MySqlCommand findcmd = new(@"
            SELECT
                coins.count, users.balance 
            FROM
                Coins coins
            JOIN 
                Users users 
            ON 
                coins.userId = users.id
            WHERE 
                userId = @userId 
            AND 
                coinId = @coinId
            ", connection);
            findcmd.Parameters.AddWithValue("@coinId", param.coinId);
            findcmd.Parameters.AddWithValue("@id", param.id);
            using (MySqlDataReader reader2 = cmd.ExecuteReader())
            {
                reader2.Read();
                count = Convert.ToInt16(reader2["count"]);
                balance = Convert.ToInt16(reader2["balance"]);
            }
            MySqlCommand pricecmd = new(@"
            SELECT 
                currentPrice 
            FROM     
                Coin 
            WHERE 
                id=@coinId
            ", connection);
            pricecmd.Parameters.AddWithValue("@coinId", param.coinId);
            using (MySqlDataReader reader3 = cmd.ExecuteReader())
            {
                reader3.Read();
                coinName = reader3["coinName"].ToString();
            }
            MySqlCommand calccmd = new(@"
            UPDATE 
                Users 
            SET
                balance = @balance
            UPDATE 
                Wallet 
            SET 
                @coinName = @coinBalance
            UPDATE 
                Wallet 
            SET 
                total =  P_WAP+P_MUT+P_APP+P_PKNU+P_PUS+P_PUFS
            ", connection);
            calccmd.Parameters.AddWithValue("@coinName", coinName);
            calccmd.Parameters.AddWithValue("@balance", balance - count * currentPrice);
            calccmd.Parameters.AddWithValue("@coinBalance", currentPrice * count);
            calccmd.ExecuteNonQuery();
            return Ok("매수체결");
        }

    }
    [HttpGet("ranking")]
    public IActionResult getRanking()
    {
        using (connection)
        {
            connection.Open();
            MySqlCommand cmd = new(@"
            SELECT
                users.balance + wallet.Total as sum
            FROM 
                Users users
            JOIN 
                Wallet wallet 
            ON 
                users.id =  wallet.userId
            ORDER BY 
                sum 
            DESC     
                LIMIT 3
            ", connection);
            var list = new List<int>();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(Convert.ToInt32(reader["sum"]));
                }
            }
            MySqlCommand cmd2 = new(@"
          SELECT 
            users.balance + wallet.Total 
          AS 
            sum
        FROM 
            Users users
        JOIN 
            Wallet wallet  on users.id =  wallet.userId
        ORDER BY 
            sum 
        ASC
            LIMIT 1
            ", connection);
            using (MySqlDataReader reader2 = cmd2.ExecuteReader())
            {
                while (reader2.Read())
                {
                    list.Add(Convert.ToInt32(reader2["sum"]));
                }
            }
            return Ok(list);
        }

    }
    [HttpGet("rate")]
    public IActionResult getRate([FromQuery] GetRate param)
    {
        using (connection)
        {
            connection.Open();
            MySqlCommand cmd = new(@"
           select admin from Users
            where id = @id
            and exists (
            select 1 from Users
            where id = @id)
            ", connection);
            cmd.Parameters.AddWithValue("@id", param.id);

            int rate = 0;
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {

                while (reader.Read())
                {
                    if (Convert.ToBoolean(reader["admin"]) == false)
                    {
                        return StatusCode(400, "관리자 권한이 필요합니다.");
                    }
                }
            }

            MySqlCommand cmd2 = new(@"
          SELECT 
            nextrate
          FROM 
            Coin 
          WHERE 
            id = @coinId
         ", connection);
            cmd2.Parameters.AddWithValue("@coinId", param.coin);
            using (MySqlDataReader reader2 = cmd2.ExecuteReader())
            {
                while (reader2.Read())
                {
                    rate = Convert.ToInt16(reader2["nextrate"]);
                }
            }
            return Ok(rate);
        }
    }
    [HttpGet("ranking/all")]
    public IActionResult getRankingAll()
    {
        using (connection)
        {
            connection.Open();
            MySqlCommand cmd = new(@"
            SELECT
                users.balance + wallet.Total as sum
            FROM 
                Users users
            JOIN 
                Wallet wallet 
            ON 
                users.id =  wallet.userId
            ORDER BY 
                sum 
            DESC     
            ", connection);
            var list = new List<int>();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(Convert.ToInt32(reader["sum"]));
                }
                return Ok(list);
            }
            
        }

    }
    [HttpPost("rate")]
    public IActionResult updateRate([FromBody] GetRate param)
    {
        using (connection)
        {
            connection.Open();
            MySqlCommand cmd = new(@"
           select admin from Users
            where id = @id
            and exists (
            select 1 from Users
            where id = @id)
            ", connection);
            cmd.Parameters.AddWithValue("@id", param.id);

            int rate = 0;
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {

                while (reader.Read())
                {
                    if (Convert.ToBoolean(reader["admin"]) == false)
                    {
                        return StatusCode(400, "관리자 권한이 필요합니다.");
                    }
                }
            }

            MySqlCommand cmd2 = new(@"
          UPDATE 
                Coin 
          SET 
                rate=@rate
           WHERE 
                id = @coinId
         ", connection);
            cmd2.Parameters.AddWithValue("@coinId", param.coin);
            cmd2.ExecuteNonQuery();
            return Ok("성공하셨습니다.");
        }

    }
    [HttpGet("time")]
    public IActionResult getTime(int id)
    {
        using (connection)
        {
            connection.Open();
            MySqlCommand cmd = new(@"
           select admin from Users
            where id = @id
            and exists (
            select 1 from Users
            where id = @id)
            ", connection);
            cmd.Parameters.AddWithValue("@id", id);

             var time = new List<DateTime>();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {

                while (reader.Read())
                {
                    if (Convert.ToBoolean(reader["admin"]) == false)
                    { return StatusCode(400, "관리자 권한이 필요합니다.");
                        Console.WriteLine(Convert.ToBoolean(reader["admin"]));
                    }
                }
            }
                   
                MySqlCommand cmd2 = new(@"
                        SELECT
                                time
                        FROM 
                                Time
                        ", connection);
            using (MySqlDataReader reader2 = cmd2.ExecuteReader())
            {

                while (reader2.Read())
                {
                    time.Add(Convert.ToDateTime(reader2["time"].ToString()));
                }
                return Ok(time);
            }
           
           
           
        }
    }

 [HttpPost("time")]
public IActionResult postTime(int id)
    {
        using (connection)
        {
            connection.Open();
            MySqlCommand cmd = new(@"
           select admin from Users
            where id = @id
            and exists (
            select 1 from Users
            where id = @id)
            ", connection);
            cmd.Parameters.AddWithValue("@id", id);

            int rate = 0;
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {

                while (reader.Read())
                {
                    if (Convert.ToBoolean(reader["admin"]) == false || reader["admin"]==System.DBNull.Value)
                    {
                        return StatusCode(400, "관리자 권한이 필요합니다.");
                    }
                }
            }

            var time = new List<DateTime>();
            MySqlCommand cmd2 = new(@"
          INSER INTO
                Time 
          VALUES 
                 @time
         ", connection);
            using (MySqlDataReader reader2 = cmd2.ExecuteReader())
            {

                while (reader2.Read())
                {
                    time.Add(Convert.ToDateTime(reader2["time"].ToString()));
                }
                return Ok(time);
            }
           
        }
    }
[HttpDelete("time")]
public IActionResult deleteTime(int id)
    {
        using (connection)
        {
            connection.Open();
            MySqlCommand cmd = new(@"
           select admin from Users
            where id = @id
            and exists (
            select 1 from Users
            where id = @id)
            ", connection);
            cmd.Parameters.AddWithValue("@id", id);

            int rate = 0;
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {

                while (reader.Read())
                {
                    if (Convert.ToBoolean(reader["admin"]) == false || reader["admin"]==System.DBNull.Value)
                    {
                        return StatusCode(400, "관리자 권한이 필요합니다.");
                    }
                }
            }

            var time = new List<DateTime>();
            MySqlCommand cmd2 = new(@"
          INSER INTO
                Time 
          VALUES 
                 @time
         ", connection);
            using (MySqlDataReader reader2 = cmd2.ExecuteReader())
            {

                while (reader2.Read())
                {
                    time.Add(Convert.ToDateTime(reader2["time"].ToString()));
                }
                return Ok(time);
            }
           
        }
    }

}



