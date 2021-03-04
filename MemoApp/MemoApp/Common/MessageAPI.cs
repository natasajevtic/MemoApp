using System.Collections.Generic;

namespace MemoApp.Common
{
    public static class MessageAPI
    {
        public static List<ErrorResult> InternalServerError = new List<ErrorResult>
        {
            new ErrorResult
            {
                ErrorCode = "Internal server error",
                ErrorDescription = "Internal server error."
            }
        };
        public static List<ErrorResult> ListOfLocationsIsEmpty = new List<ErrorResult>
        {
            new ErrorResult
            {
                ErrorCode = "ListOfLocationsIsEmpty",
                ErrorDescription = "List of locations is empty."
            }
        };
        public static List<ErrorResult> SessionAlreadyActive = new List<ErrorResult>
        {
            new ErrorResult
            {
                ErrorCode = "SessionAlreadyActive",
                ErrorDescription = "User already has active session."
            }
        };
        public static List<ErrorResult> SessionNotActive = new List<ErrorResult>
        {
            new ErrorResult
            {
                ErrorCode = "SessionNotActive",
                ErrorDescription = "This session is already stopped."
            }
        };
        public static List<ErrorResult> SessionDoesntExist = new List<ErrorResult>
        {
            new ErrorResult
            {
                ErrorCode = "SessionDoesntExist",
                ErrorDescription = "This session does not exist."
            }
        };
        public static List<ErrorResult> SessionDoesntBelongToUser = new List<ErrorResult>
        {
            new ErrorResult
            {
                ErrorCode = "SessionDoesntBelongToUser",
                ErrorDescription = "This session doesn't belong to this user."
            }
        };
        public static List<ErrorResult> StopMustBeGreaterThanStart = new List<ErrorResult>
        {
            new ErrorResult
            {
                ErrorCode = "StopMustBeGreaterThanStart",
                ErrorDescription = "Stop must be greater than start."
            }
        };
        public static List<ErrorResult> AlreadyDeclaredAsPositive = new List<ErrorResult>
        {
            new ErrorResult
            {
                ErrorCode = "AlreadyDeclaredAsPositive",
                ErrorDescription = "This user has been already declared as positive."
            }
        };
        public static List<ErrorResult> AccountLockedOut = new List<ErrorResult>
        {
            new ErrorResult
            {
                ErrorCode = "AccountLockedOut",
                ErrorDescription = "Account locked out."
            }
        };
        public static List<ErrorResult> InvalidAttempt = new List<ErrorResult>
        {
            new ErrorResult
            {
                ErrorCode = "InvalidAttempt",
                ErrorDescription = "Invalid attempt."
            }
        };
        public static List<ErrorResult> InvalidJSON = new List<ErrorResult>
        {
            new ErrorResult
            {
                ErrorCode = "InvalidJSON",
                ErrorDescription = "Invalid number of elements in JSON request"
            }
        };
        public static List<ErrorResult> ConfirmationCodeDoesntExist = new List<ErrorResult>
        {
            new ErrorResult
            {
                ErrorCode = "ConfirmationCodeDoesntExist",
                ErrorDescription = "Confirmation code does not exist in our system."
            }
        };

        public static List<ErrorResult> NegativeResultOfTest = new List<ErrorResult>
        {
            new ErrorResult
            {
                ErrorCode = "NegativeResultOfTest",
                ErrorDescription = "Test result is negative."
            }
        };

        public static List<ErrorResult> ResultOfTestDoesntExist = new List<ErrorResult>
        {
            new ErrorResult
            {
                ErrorCode = "ResultOfTestDoesntExist",
                ErrorDescription = "Result of test does not exist."
            }
        };

        public static List<ErrorResult> ConfirmationCodeAlreadyExists = new List<ErrorResult>
        {
            new ErrorResult
            {
                ErrorCode = "ConfirmationCodeAlreadyExists",
                ErrorDescription = "Confirmation Code has been already used."
            }
        };
        public static List<ErrorResult> UnsuccessfulAuthentication = new List<ErrorResult>
        {
            new ErrorResult
            {
                ErrorCode = "UnsuccessfulAuthentication",
                ErrorDescription = "Unsuccessful authentication."
            }
        };
    }
}
