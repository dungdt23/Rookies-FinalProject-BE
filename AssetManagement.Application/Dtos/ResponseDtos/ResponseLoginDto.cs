namespace AssetManagement.Application.Dtos.ResponseDtos
{
    public class ResponseLoginDto
    {
        public string TokenType { get; set; } = "Bearer";
        public string Token { get; set; }
    }
}
