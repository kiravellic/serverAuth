using System.Collections.Generic;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;

namespace Server.Auth.BLL;

public class GenerateJwt
{
    private readonly IJwtEncoder _jwtEncoder;
    public GenerateJwt()
    {
        // JWT specific initialization.
        var algorithm = new HMACSHA256Algorithm();
        var serializer = new JsonNetSerializer();
        var base64Encoder = new JwtBase64UrlEncoder();
        _jwtEncoder = new JwtEncoder(algorithm, serializer, base64Encoder);
    }
    public string IssuingJWT(string user)
    {
        Dictionary<string, object> claims = new Dictionary<string, object>
        {
            // JSON representation of the user Reference with ID and display name
            { "username", user },
 
            // TODO: Add other claims here as necessary; maybe from a user database
            { "role", "admin"}
        };

        string token = _jwtEncoder.Encode(claims, "Your Secret Securtity key string"); // Put this key in config
        return token;
    }
}