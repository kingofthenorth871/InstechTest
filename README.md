ClaimsController API Documentation

The ClaimsController is responsible for handling all HTTP requests related to claim operations in the application. It provides methods to retrieve, create, and delete claims as part of the insurance claims handling module.
Dependencies

    ILogger<ClaimsController>: Used for logging activities within the controller.
    IClaimsService: Service layer interface for managing claim entities.
    IAuditsService: Service layer interface for auditing claim activities.

API Endpoints
GET /Claims

Retrieves a list of all claims in the system.
Parameters

None
Returns

    IEnumerable<Claim>: A list of Claim objects.
    Example:
    [
    {
        "Id": "abc123",
        "CoverId": "cover456",
        "Created": "2024-01-01T00:00:00Z",
        "Name": "John Doe",
        "Type": "Collision",
        "DamageCost": 5000
    }
]


POST /Claims

Creates a new claim in the system.
Parameters

    Claim (body): A Claim object representing the new claim.

Returns

    ActionResult: HTTP Status Code indicating success or failure.
    Example:

{
    "Id": "new123",
    "CoverId": "cover789",
    "Created": "2024-01-02T00:00:00Z",
    "Name": "Jane Doe",
    "Type": "Fire",
    "DamageCost": 10000
}

Errors

    500 Internal Server Error: If an exception occurs during processing.

DELETE /Claims/{id}

Deletes a claim identified by its unique ID.
Parameters

    id (URL parameter): The unique identifier of the claim to be deleted.

Returns

    void: Response is typically empty but indicates success with a 204 status code when successful.

GET /Claims/{id}

Retrieves a specific claim identified by its unique ID.
Parameters

    id (URL parameter): The unique identifier of the claim to be fetched.

Returns

    Claim: The requested claim object.
    Example:

{
    "Id": "abc123",
    "CoverId": "cover456",
    "Created": "2024-01-01T00:00:00Z",
    "Name": "John Doe",
    "Type": "Collision",
    "DamageCost": 5000
}

Error Handling

All endpoints incorporate error handling that captures and logs backend exceptions. Errors are returned as HTTP status codes, with 500 typically indicating an internal server error.

Logging
All significant actions within the controller are logged for audit and debugging purposes, especially creation and deletion of claims.


# CoversController Documentation

The `CoversController` manages operations related to insurance covers within the system. It provides endpoints for computing premiums, retrieving covers, creating new covers, and deleting existing covers.

## Endpoints

### Compute Premium

**HTTP Method:** POST

**Route:** /covers/compute

**Description:** Calculates the premium for a given cover based on the start date, end date, and cover type.

**Request Body:**
- `startDate` (DateTime): The start date of the cover.
- `endDate` (DateTime): The end date of the cover.
- `coverType` (CoverType): The type of cover.

**Response:**
- Status Code: 200 OK
- Body: The calculated premium amount.

### Get All Covers

**HTTP Method:** GET

**Route:** /covers

**Description:** Retrieves all covers stored in the system.

**Response:**
- Status Code: 200 OK
- Body: An array of Cover objects.

### Get Cover by ID

**HTTP Method:** GET

**Route:** /covers/{id}

**Description:** Retrieves a specific cover by its unique identifier.

**Request Parameter:**
- `id` (string): The unique identifier of the cover.

**Response:**
- Status Code: 200 OK
- Body: The Cover object corresponding to the provided ID.

### Create Cover

**HTTP Method:** POST

**Route:** /covers

**Description:** Creates a new cover in the system.

**Request Body:**
- `cover` (Cover): The cover object to be created. Must include start date, end date, and cover type.

**Response:**
- Status Code: 200 OK
- Body: The created Cover object, including its generated unique identifier and computed premium.

### Delete Cover

**HTTP Method:** DELETE

**Route:** /covers/{id}

**Description:** Deletes an existing cover from the system.

**Request Parameter:**
- `id` (string): The unique identifier of the cover to be deleted.

**Response:**
- Status Code: 200 OK (No content)

## Dependencies

- **ICoversService:** Interface for interacting with cover-related data and operations.
- **IAuditsService:** Interface for auditing cover-related actions.
- **ILogger:** Interface for logging messages within the controller.
- **ComputePremiumCalculator:** Utility class for computing cover premiums.

Database documentation:

1. ClaimAudits Database Table:

This table stores audit records for claims made in the system.

    Columns:
        Id (Primary Key, auto-generated integer): Unique identifier for each audit record.
        ClaimId (String, required): Identifier for the associated claim.
        Created (DateTime): Timestamp indicating when the audit record was created.
        HttpRequestType (String, required): Type of HTTP request associated with the audit.

2. CoverAudits Database Table:

This table stores audit records for coverages applied in the system.

    Columns:
        Id (Primary Key, auto-generated integer): Unique identifier for each audit record.
        CoverId (String, required): Identifier for the associated coverage.
        Created (DateTime): Timestamp indicating when the audit record was created.
        HttpRequestType (String, required): Type of HTTP request associated with the audit.

Context and Relationships:

    These audit tables serve as a log for tracking actions related to claims and coverages.
    Each audit record captures details such as the ID of the claim or coverage, timestamp of the action, and the type of HTTP request (e.g., POST, DELETE).
    There are no explicit relationships defined between these audit tables and other tables in the database. They function as standalone logs to maintain a record of relevant actions in the system.

Additional Information:

    The ClaimAudits and CoverAudits tables are designed to provide a history of interactions with claims and coverages, aiding in auditing, troubleshooting, and monitoring activities within the system.
    The Created column records the date and time when the audit record was created, enabling chronological tracking of actions.
    The HttpRequestType column specifies the type of HTTP request (e.g., POST, DELETE) associated with each audit entry, facilitating identification of the operation performed.

This documentation outlines the purpose, structure, and context of the ClaimAudits and CoverAudits tables, providing insight into their role within the database schema.






The assignment that has been solved:
# Read this first!
This repository is a template repository for our technical interview, so create your own project using this guide:

[GitHub - Creating a repository from a template](https://docs.github.com/en/repositories/creating-and-managing-repositories/creating-a-repository-from-a-template).

An alternative is to download the code and create a new repository using a different VCS provider (gitlab / Azure repos). **Do not fork this repository.**

When you have completed the tasks, please share the repository link with us. We will review your submission before the interview.

Good luck! 😊

# Prerequisites

- Your favourite IDE to code in C# 😊
- _Optional_ - an Azure Subscription. You can demo this API by hosting it in Azure. If that is not an option for you, you can run the demo having a locally running instance. If you select a different cloud provider, that is fine for us.
- `Docker` and `docker-compose`

Use `$ docker-compose up --detach` to start the containers with dependencies. The existing code base is preconfigured to work with these containers.
There are no volumes setup for any of the storage, so when you `docker-compose down` these storage media *WILL NOT BE PERSISTED*.

> **Disclaimer**
> 
> As you can see - a DB password is committed to the `appsettings.json` file. However, these secrets are just for development dependencies. If you deploy
> the application into the cloud, we expect that you make use of an alternate method of storing secrets.

# Programming Task
Complete the backend for a multi-tier application for Insurance Claims Handling.
The use case is to maintain a list of insurance claims. The user should be able to create, delete and read claims.
## Task 1
The codebase is messy:
* The controller has too much responsibility. 
* Introduce proper layering within the codebase. 
* Documentation is missing.
* ++

Adhere to the SOLID principles.

### Task 2
As you can see, the API supports some basic REST operations. But validation is missing. The basic rules are:

* Claim
  * DamageCost cannot exceed 100.000
  * Created date must be within the period of the related Cover
* Cover
  * StartDate cannot be in the past
  * total insurance period cannot exceed 1 year

## Task 3
Auditing. The basics are there, but the execution of the DB command (INSERT & DELETE) blocks the processing of the HTTP request. How can this be improved? Look into some asynchronous patterns. It is ok to introduce an Azure managed service to help you with this (ServiceBus/EventGrid/Whatever), but that is not required. Whatever you can manage to get working which is in-memory is also ok.

## Task 4
One basic test is included, please add other (mandatory) unit tests. Note: If you start on this task first, you will find it hard to write proper tests. Some refactoring of the Claims API will be needed. 

## Task 5
Cover premium computation formula evolved over time. Fellow developers were lazy and omitted all tests. Now there are a couple of bugs. Can you fix them? Can you make the computation more readable?

Desired logic: 
* Premium depends on the type of the covered object and the length of the insurance period. 
* Base day rate was set to be 1250.
* Yacht should be 10% more expensive, Passenger ship 20%, Tanker 50%, and other types 30%
* The length of the insurance period should influence the premium progressively:
  * First 30 days are computed based on the logic above
  * Following 150 days are discounted by 5% for Yacht and by 2% for other types
  * The remaining days are discounted by additional 3% for Yacht and by 1% for other types
