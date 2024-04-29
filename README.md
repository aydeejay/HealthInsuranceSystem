# HealthInsuranceSystem.Core.Services - User Service

## Overview
The `UserService` is a part of the `HealthInsuranceSystem.Core.Services` project and provides methods to interact with the `User` entity in the application. The `UserService` is responsible for performing CRUD operations on the `User` entity, such as retrieving all users, retrieving a user by policy number, and creating a new user.

The `ClaimService` is a part of the `HealthInsuranceSystem.Core.Services` project and provides methods to interact with the `claim` entity in the application. The `ClaimService` is responsible for performing CRUD operations on the `Claim` entity, such as retrieving all claims, creating claims and reviewing claims by the claim reviewer and admin.

## Getting Started

## Key Features
- Retrieves all users and claims with pagination and filtering capabilities.
- Retrieves a user by policy number.
- Creates a new user with a generated policy holder ID and hashes the user's password using a secure algorithm.
- Create claim by policy holder
- review claim

## Dependencies
- .Net 8
- AutoMapper: A object to object mapping (OOM) library for .NET.
- SQL Server
- Fluent Validation
- Microsoft.EntityFrameworkCore: A popular open-source, modern-designed, schema-agnostic data-access framework for .NET.
- Moq: A popular NUnit test framework for .NET.
- Swagger API documentation

## Authentication and Authentication
JWT Identity server authentication is used. The flow used here is called ResourceOwner password where we validate the user credentials, Client credentials and audience before generating token.
There is also claim authorization that determines the roles that can access specific endpoints

## Run Application
- You are required to have SQL Server installed. 
- You can change the database connection string based on your SQL Server installation
- There is auto migration enabled with Default Data for the admin users. (Login credentials will be provided for admin but policy holder account has to be created via postman or the swagger interface)
- After running the application, Necessary database, tables, columns and data would have been automatically created.
- You can now use the swagger page to autenticate using the National ID and the user password.
- The swagger extension will automatically add the token to your request. Please fire away.

## Test Credentials
Admin user
  NationalID - AdminNationalId
  Password - Password
Claim Processor
  NationalId - ClaimProcessorNationalId
  Password - Password

## Testing
The `UserService` class is tested using NUnit and Moq. The tests cover the functionality of the `UserService` methods, such as retrieving all users, retrieving a user by policy number, and creating a new user.

## Contributing
Contributions are welcome! If you find any issues or have any suggestions for improvements, please feel free to open an issue or submit a pull request.
