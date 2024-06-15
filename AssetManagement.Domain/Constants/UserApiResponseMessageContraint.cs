namespace AssetManagement.Domain.Constants;

public static class UserApiResponseMessageContraint
{
    public const string UserCreateSuccess = "User created successfully";
    public const string UserCreateFail = "There something went wrong while creating user, please try again later";
    public const string UserNotFound = "No user founded";
    public const string UserUpdateSuccess = "User updated successfully";
    public const string UserUpdateFail = "There something went wrong while updating user, please try again later";

    public const string UserLoginWrongPasswordOrUsername = "UserName or password incorrect";

    public const string UserLoginSuccess = "User login successfully";
    
}
