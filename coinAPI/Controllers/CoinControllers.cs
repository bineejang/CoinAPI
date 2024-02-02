using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Coin.Models;
using MySql.Data.MySqlClient;
using Microsoft.Data.SqlClient;
using Org.BouncyCastle.Asn1.Cms;
using Microsoft.EntityFrameworkCore.Query;
using System.Data;
using System.Runtime.Serialization;
using System.Runtime.ConstrainedExecution;

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
    [HttpGet("userInfo")]
    public IActionResult getUserInfo( int id)
    {
        try
        {
            
        
        using (connection)
        {
            if (id == 0 || id == null)
            {
                return StatusCode(400, "아이디 없음");
            }
            else
            {
                connection.Open();
                MySqlCommand cmd = new(@"
                select * from 
                Users 
                WHERE id = @id;
            ", connection);
            cmd.Parameters.AddWithValue("@id", id);
                
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

                }
                return Ok(profile);
            }
        }
        }
        catch (System.Exception)
        {
            return StatusCode(400,"시스템 오류");
            throw;
        }
    }
    [HttpPost("login")]
    public IActionResult login([FromBody] login param)
    {
            try{
        using (connection)
        {
            if (param.userId == "" || param.password == "")
            {
                return StatusCode(400, "아이디와 비번을  확인해주세요");
            }
            else
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

                }
                return Ok(profile);
            }
        }
        }
         catch (System.Exception)
        {
            return StatusCode(400,"시스템 오류");
            throw;
        }
    }
    [HttpGet("holdings/{id}")]
    public IActionResult GetHoldings(int id)
    {   
        try{
        using (connection)
        {
            connection.Open();
            var Wallet = new List<Wallet>(0);
            int wapt = 0, appt = 0, must = 0, pknut = 0, pust = 0, bufst = 0, totalt = 0;
            var list = new List<int>(0);
            MySqlCommand cmd1 = new(@"
               select IFNULL(count,0) from MAP.Users users
                join MAP.Coins coins on users.id = coins.userId
                join MAP.Coin coin on coins.coinId = coin.id
                where users.id = @id and coins.coinId = 1
            ", connection);
            cmd1.Parameters.AddWithValue("@id", id);
            cmd1.Parameters["@id"].Value = id;
            using (MySqlDataReader reader = cmd1.ExecuteReader())
            {
                while (reader.Read())
                {
                    
                    list.Add(
                        Convert.ToInt32(reader["IFNULL(count,0)"])
                    );


                    wapt = Convert.ToInt32(reader["IFNULL(count,0)"]);

                }
            }

            MySqlCommand cmd2 = new(@"
                select IFNULL(count,0) from MAP.Users users
                join MAP.Coins coins on users.id = coins.userId
                join MAP.Coin coin on coins.coinId = coin.id
                where users.id = @id and coins.coinId = 2
            ", connection);
            cmd2.Parameters.AddWithValue("@id", id);
            cmd2.Parameters["@id"].Value = id;
            using (MySqlDataReader reader2 = cmd2.ExecuteReader())
            {
                while (reader2.Read())
                {


                    appt = Convert.ToInt32(reader2["IFNULL(count,0)"]);

                }
            }

            MySqlCommand cmd3 = new(@"
                select IFNULL(count,0) from MAP.Users users
                join MAP.Coins coins on users.id = coins.userId
                join MAP.Coin coin on coins.coinId = coin.id
                where users.id = @id and coins.coinId = 3
            ", connection);
            cmd3.Parameters.AddWithValue("@id", id);
            cmd3.Parameters["@id"].Value = id;
            using (MySqlDataReader reader3 = cmd3.ExecuteReader())
            {
                while (reader3.Read())
                {


                    must = Convert.ToInt32(reader3["IFNULL(count,0)"]);

                }
            }

            MySqlCommand cmd4 = new(@"
                select IFNULL(count,0) from MAP.Users users
                join MAP.Coins coins on users.id = coins.userId
                join MAP.Coin coin on coins.coinId = coin.id
                where users.id = @id and coins.coinId = 4
            ", connection);
            cmd4.Parameters.AddWithValue("@id", id);
            cmd4.Parameters["@id"].Value = id;
            using (MySqlDataReader reader4 = cmd4.ExecuteReader())
            {
                while (reader4.Read())
                {


                    pknut = Convert.ToInt32(reader4["IFNULL(count,0)"]);

                }
            }

            MySqlCommand cmd5 = new(@"
                select IFNULL(count,0) from MAP.Users users
                join MAP.Coins coins on users.id = coins.userId
                join MAP.Coin coin on coins.coinId = coin.id
                where users.id = @id and coins.coinId = 5
            ", connection);
            cmd5.Parameters.AddWithValue("@id", id);
            cmd5.Parameters["@id"].Value = id;
            using (MySqlDataReader reader5 = cmd5.ExecuteReader())
            {
                while (reader5.Read())
                {


                    pust = Convert.ToInt32(reader5["IFNULL(count,0)"]);

                }
            }

            MySqlCommand cmd6 = new(@"
                select IFNULL(count,0) from MAP.Users users
                join MAP.Coins coins on users.id = coins.userId
                join MAP.Coin coin on coins.coinId = coin.id
                where users.id = @id and coins.coinId = 6
            ", connection);
            cmd6.Parameters.AddWithValue("@id", id);
            cmd6.Parameters["@id"].Value = id;
            using (MySqlDataReader reader6 = cmd6.ExecuteReader())
            {
                while (reader6.Read())
                {


                    bufst = Convert.ToInt32(reader6["IFNULL(count,0)"]);

                }
            }
            MySqlCommand cmd7 = new(@"
                SELECT
                users.id,(users.balance + SUM(wallet.Total)) as sum
            FROM 
                Users users
            JOIN 
                Wallet wallet 
            ON 
                users.id =  wallet.userId
           where
                users.id =@id
            ;
            ", connection);
            cmd7.Parameters.AddWithValue("@id", id);
            cmd7.Parameters["@id"].Value = id;
            using (MySqlDataReader reader7 = cmd7.ExecuteReader())
            {
                while (reader7.Read())
                {   
                    totalt = Convert.ToInt32(reader7["sum"]);
                }
            }
            Wallet.Add(new Wallet
            {
                wap = wapt,
                app = appt,
                mus = must,
                pknu = pknut,
                pus = pust,
                bufs = bufst,
                total = totalt
            });
            return Ok(Wallet);
        }
        }
         catch (System.Exception)
        {
            return StatusCode(400,"시스템 오류");
            throw;
        }
    }

    [HttpGet("coin")]
    public IActionResult getCoins()
    {   
        try{
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
         catch (System.Exception)
        {
            return StatusCode(400,"시스템 오류");
            throw;
        }
    }
    [HttpPost("coin/sell")]
    public IActionResult CoinSell([FromBody] Contract param)
    {   
        try
        {
        using (connection)
        {
            connection.Open();
            int count = 0;
            int balance = 0;
            int currentPrice = 0;
            string coinName = "";
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
                while (reader1.Read())
                {
                    count = Convert.ToInt32(reader1["count"]);
                }
            }
            if (count < param.count)
            {
                return StatusCode(400, "수량이 부족합니다.");
            }
            else
            {
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
                IFNULL(users.balance,0) 
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
                findcmd.Parameters.AddWithValue("@userId", param.id);
                using (MySqlDataReader reader2 = findcmd.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        
                        balance = Convert.ToInt32(reader2["IFNULL(users.balance,0)"]);
                    }
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
                using (MySqlDataReader reader3 = pricecmd.ExecuteReader())
                {
                    while (reader3.Read())
                    {
                        currentPrice = Convert.ToInt32(reader3["currentPrice"]);
                    }
                }
                MySqlCommand calccmd = new(@"
            UPDATE 
                Users 
            SET
                balance = @balance
                WHERE id = @userId
            ", connection);


                calccmd.Parameters.AddWithValue("@balance", balance + param.count * currentPrice);
                calccmd.Parameters.AddWithValue("@userId", param.id);
                calccmd.ExecuteNonQuery();
                int total = 0;
                MySqlCommand findtotalcmd = new(@"
            SELECT 
                Total 
            FROM     
                Wallet 
            WHERE 
                totalId=@totalId
                and userId = @userId;
            ", connection);
                findtotalcmd.Parameters.AddWithValue("@totalId", param.coinId);
                findtotalcmd.Parameters.AddWithValue("@userId", param.id);
                using (MySqlDataReader readerfindtotal = findtotalcmd.ExecuteReader())
                {
                    while (readerfindtotal.Read())
                    {
                        total = Convert.ToInt32(readerfindtotal["Total"]);
                    }
                }
                MySqlCommand totalcmd = new(@"
            UPDATE 
                Wallet 
            SET
                Total = @total
                WHERE userId = @userId
                and totalId = @totalId
            ", connection);

                totalcmd.Parameters.AddWithValue("@total", count * currentPrice - param.count * currentPrice);
                totalcmd.Parameters.AddWithValue("@userId", param.id);
                totalcmd.Parameters.AddWithValue("@totalId", param.coinId);
                totalcmd.ExecuteNonQuery();
                return Ok("매도체결");
            }
        }
        }
        catch (System.Exception)
        {
            return StatusCode(400,"시스템 오류입니다.");
            throw;
        }
        
        

    }
    [HttpPost("coin/buy")]
    public IActionResult Coinbuy([FromBody] Contract param)
    {   try{
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
                while (reader1.Read())
                {
                    count = Convert.ToInt32(reader1["count"]);
                }
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
            setCountcmd.Parameters.AddWithValue("@count", count + param.count);
            setCountcmd.ExecuteNonQuery();
            MySqlCommand findcmd = new(@"
            SELECT
                IFNULL(users.balance,0)
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
            findcmd.Parameters.AddWithValue("@userId", param.id);
            using (MySqlDataReader reader2 = findcmd.ExecuteReader())
            {
                while (reader2.Read())
                {
                    
                    balance = Convert.ToInt32(reader2["IFNULL(users.balance,0)"]);
                    
                }
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
            using (MySqlDataReader reader3 = pricecmd.ExecuteReader())
            {
                while (reader3.Read())
                {
                    currentPrice = Convert.ToInt32(reader3["currentPrice"]);
                    

                }
            }

            MySqlCommand calccmd = new(@"
            UPDATE 
                Users 
            SET
                balance = @balance
                WHERE id = @userId

            ", connection);
            calccmd.Parameters.AddWithValue("@balance", balance - param.count * currentPrice);
            
            calccmd.Parameters.AddWithValue("@userId", param.id);
            calccmd.ExecuteNonQuery();
            int total = 0;
            MySqlCommand findtotalcmd = new(@"
            SELECT 
                Total 
            FROM     
                Wallet 
            WHERE 
                totalId=@totalId
                and userId = @userId;
            ", connection);
            findtotalcmd.Parameters.AddWithValue("@totalId", param.coinId);
            findtotalcmd.Parameters.AddWithValue("@userId", param.id);
            using (MySqlDataReader readerfindtotal = findtotalcmd.ExecuteReader())
            {
                while (readerfindtotal.Read())
                {
                    total = Convert.ToInt32(readerfindtotal["Total"]);
                }
            }
            MySqlCommand totalcmd = new(@"
            UPDATE 
                Wallet 
            SET
                Total = @total
                WHERE userId = @userId
                and totalId = @totalId
            ", connection);

            totalcmd.Parameters.AddWithValue("@total", count * currentPrice + param.count * currentPrice);
            totalcmd.Parameters.AddWithValue("@userId", param.id);
            totalcmd.Parameters.AddWithValue("@totalId", param.coinId);
            totalcmd.ExecuteNonQuery();
            return Ok("매수체결");
        }
    }
         catch (System.Exception)
        {
            return StatusCode(400,"시스템 오류");
            throw;
        }

    }
    [HttpGet("ranking")]
    public IActionResult getRanking()
    {   try{
        using (connection)
        {
            connection.Open();
            MySqlCommand cmd = new(@"
           SELECT
                *,(users.balance + SUM(wallet.Total)) as sum
            FROM 
                Users users
            JOIN 
                Wallet wallet 
            ON 
                users.id =  wallet.userId
            group by wallet.userId
            ORDER BY 
                sum 
            DESC
                LIMIT 3;
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
                *,users.balance + SUM(wallet.Total) as sum
            FROM 
                Users users
            JOIN 
                Wallet wallet 
            ON 
                users.id =  wallet.userId
            group by wallet.userId
            ORDER BY 
                sum 
        ASC
            LIMIT 1;
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
         catch (System.Exception)
        {
            return StatusCode(400,"시스템 오류");
            throw;
        }

    }
    [HttpGet("rate")]
    public IActionResult getRate([FromQuery] GetRate param)
    {   
        try{
        using (connection)
        {
            connection.Open();
            MySqlCommand cmd = new(@"
           select IFNULL(admin,0) from Users
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

                    if (Convert.ToBoolean(reader["IFNULL(admin,0)"]) == false)

                    {
                        return StatusCode(400, "관리자 권한이 필요합니다.");

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
         catch (System.Exception)
        {
            return StatusCode(400,"시스템 오류");
            throw;
        }
        
    }

    [HttpGet("ranking/all")]
    public IActionResult getRankingAll()
    {   try{
        using (connection)
        {
            connection.Open();
            MySqlCommand cmd = new(@"
            SELECT
                *,(users.balance + SUM(wallet.Total)) as sum
            FROM 
                Users users
            JOIN 
                Wallet wallet 
            ON 
                users.id =  wallet.userId
            group by wallet.userId
            ORDER BY 
                sum 
            DESC;
            ", connection);
            var list = new List<RankingAll>();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new RankingAll
                    {
                        balance = Convert.ToInt32(reader["balance"]),
                        id = Convert.ToInt16(reader["id"]),
                        name = reader["name"].ToString(),
                        crew = reader["crew"].ToString(),
                        type = reader["type"].ToString(),
                        club = reader["club"].ToString(),
                        phonenum = reader["phonenum"].ToString(),
                        total = Convert.ToInt32(reader["sum"]),
                    });
                }
                return Ok(list);
            }

        }
    }
         catch (System.Exception)
        {
            return StatusCode(400,"시스템 오류");
            throw;
        }

    }
    [HttpPost("rate")]
    public IActionResult updateRate([FromBody] GetRate param)
    {   try{
        using (connection)
        {
            bool admin = false;
            connection.Open();
            MySqlCommand cmd = new(@"
           select IFNULL(admin,null) from Users
            where id = @id
            and exists (
            select 1 from Users
            where id = @id)
            ", connection);
            cmd.Parameters.AddWithValue("@id", param.id);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {

                while (reader.Read())
                {
                    admin = Convert.ToBoolean(reader["IFNULL(admin,null)"]);
                }
            }
            if (admin == true)
            {

                MySqlCommand cmd2 = new(@"
          UPDATE 
                Coin 
          SET 
                nextrate=@rate
           WHERE 
                id = @coinId
         ", connection);
                cmd2.Parameters.AddWithValue("@coinId", param.coin);
                cmd2.Parameters.AddWithValue("@rate", param.rate);
                cmd2.ExecuteNonQuery();
                return Ok("성공하셨습니다.");
            }
        
            
            else
            {
                return StatusCode(400, "관리자 권한이 필요합니다.");
            }
        }
    }
         catch (System.Exception)
        {
            return StatusCode(400,"시스템 오류");
            throw;
        }
        
    }
    [HttpGet("time")]
    public IActionResult getTime()
    {   try{
        using (connection)
        {
           
            connection.Open();
            

                var time = new List<GetTime>();
                MySqlCommand cmd2 = new(@"
                SELECT
                    id,DATE_FORMAT(savedtime,'%H:%i') AS time
                FROM 
                    Time 
                ORDER BY time ASC ;
                               
                        ", connection);
                using (MySqlDataReader reader2 = cmd2.ExecuteReader())
                {
                    // timeElement=TimeOnly.FromDateTime(Convert.ToDateTime(reader2["time"].ToString())),
                    // timeElement=Convert.ToDateTime(reader2["time"].ToString()),
                    while (reader2.Read())
                    {
                        time.Add(new GetTime
                        {
                            timeElement = Convert.ToDateTime(reader2["time"].ToString()),
                            timeId = Convert.ToInt32(reader2["id"])
                        });
                    }

                }
                return Ok(time);
            }
    }
             catch (System.Exception)
        {
            return StatusCode(400,"시스템 오류");
            throw;
        }
            
        }
    

    [HttpPost("time")]
    public IActionResult postTime([FromQuery] GetTime param)
    {   try{
        using (connection)
        {
            bool admin = false;
            connection.Open();
            MySqlCommand cmd = new(@"
           select IFNULL(admin,null) from Users
            where id = @id
            and exists (
            select 1 from Users
            where id = @id)
            ", connection);
            cmd.Parameters.AddWithValue("@id", param.id);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {

                while (reader.Read())
                {
                    admin = Convert.ToBoolean(reader["IFNULL(admin,null)"]);
                }
            }
            if (admin == true)
            {
                var time = new List<DateTime>();
                MySqlCommand cmd2 = new(@"
         INSERT INTO 
            `Time`(savedtime) 
            VALUES
                (@time)

         ", connection);
                cmd2.Parameters.AddWithValue("@time", Convert.ToDateTime(param.timeElement).ToString("O"));
                cmd2.ExecuteNonQuery();

                return Ok("저장되었습니다.");
            }
            else
            {
                return StatusCode(400, "관리자 권한이 필요합니다.");
            }

        }
    }
         catch (System.Exception)
        {
            return StatusCode(400,"시스템 오류");
            throw;
        }
    }
    [HttpDelete("time")]
    public IActionResult deleteTime([FromQuery] GetTime param)
    {   
        try{
        using (connection)
        {
            bool admin = false;
            connection.Open();
            MySqlCommand cmd = new(@"
           select IFNULL(admin,null) from Users
            where id = @id
            and exists (
            select 1 from Users
            where id = @id)
            ", connection);
            cmd.Parameters.AddWithValue("@id", param.id);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {

                while (reader.Read())
                {
                    admin = Convert.ToBoolean(reader["IFNULL(admin,null)"]);
                }
            }
            if (admin == true)
            {

                var time = new List<DateTime>();
                MySqlCommand cmd2 = new(@"
          DELETE FROM 
            `Time` 
            WHERE 
                `id`=@id
         ", connection);
                cmd2.Parameters.AddWithValue("@id", param.timeId);
                cmd2.ExecuteNonQuery();
                return Ok("삭제되었습니다.");
            }
            else
            {
                return StatusCode(400, "관리자 권한이 필요합니다.");
            }

        }
        }
         catch (System.Exception)
        {
            return StatusCode(400,"시스템 오류");
            throw;
        }
    }

}



