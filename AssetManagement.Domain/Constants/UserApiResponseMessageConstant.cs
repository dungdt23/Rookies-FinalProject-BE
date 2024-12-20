namespace AssetManagement.Domain.Constants;

public static class UserApiResponseMessageConstant
{
    public const string UserCreateSuccess = "User created successfully";
    public const string UserCreateFail = "There something went wrong while creating user, please try again later";
    public const string UserNotFound = "No user founded";
    public const string TypeNotFound = "No type founded";
    public const string UserUpdateSuccess = "User updated successfully";
    public const string UserUpdateFail = "There something went wrong while updating user, please try again later";
    public const string UserLoginWrongPasswordOrUsername = "Username or password is incorrect. Please try again";
    public const string UserLoginSuccess = "User login successfully";
    public const string DisabledUser = "Your account has been disabled for admin. You can contact admin for solution!";
    public const string OldPasswordWrong = "Password is incorrect";
    public const string MatchOldPassword = "New password should not be the same as old password";
	public const string ChangePasswordSuccess = "Password changed successfully";
	public const string ChangePasswordFail = "Password changed failed";
}
