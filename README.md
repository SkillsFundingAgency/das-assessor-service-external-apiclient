# ![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png) Digital Apprenticeships Service

#  End Point Assessor Certification API (das-assessor-service-external-apiclient)

![Build Status](https://sfa-gov-uk.visualstudio.com/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/831/badge)

## License

Licensed under the [MIT license](https://github.com/SkillsFundingAgency/das-assessor-service-external-apiclient/blob/master/LICENSE)

## Developer Setup

### Requirements

- Install [Visual Studio 2017](https://www.visualstudio.com/downloads/) with these workloads:
    - ASP.NET and web development
    - .NET desktop development
	- .NET Core 2.1 SDK
- Create an account on the [Developer Portal](https://developers.apprenticeships.sfa.bis.gov.uk/)
	- Obtain External API Subscription Key and Base Address
	- Can also be used to access the current Swagger Documentation

### Open the solution

- Open the solution
- Set SFA.DAS.AssessorService.ExternalApi.Client as the start-up project
- Update the API Subscription Key and Base Address
- Running the solution will launch the desktop application

####  SFA.DAS.AssessorService.ExternalApi.Client

A simple Windows Presentation Foundation (WPF) client.
- Reads in CSV formatted files
	- There are examples in the following solution folder: [CsvFiles](https://github.com/SkillsFundingAgency/das-assessor-service-external-apiclient/tree/master/src/SFA.DAS.AssessorService.ExternalApi.Examples/CsvFiles)

####  SFA.DAS.AssessorService.ExternalApi.Examples

Example code should you wish to implement your own client:
- [Program.cs](https://github.com/SkillsFundingAgency/das-assessor-service-external-apiclient/blob/master/src/SFA.DAS.AssessorService.ExternalApi.Examples/Program.cs) - basic client
- [ProgramCsv.cs](https://github.com/SkillsFundingAgency/das-assessor-service-external-apiclient/blob/master/src/SFA.DAS.AssessorService.ExternalApi.Examples/ProgramCsv.cs) - CSV enabled client

####  SFA.DAS.AssessorService.ExternalApi.Core

Core functionality to interact with the External API.

####  SFA.DAS.AssessorService.ExternalApi.Core.Tests

Unit and mocked Integration tests.

## Beta Testing

This is a new service - your feedback will help us to improve it 
https://www.smartsurvey.co.uk/s/certification-API/


## Sample Scenarios
For details see the online Swagger documentation in the [Developer Portal](https://developers.apprenticeships.sfa.bis.gov.uk/).


## Request Certificate for Apprenticeships 

### 1.   Check Certificate (Optional) 

#### To check on the status of a learner's certificate for a specific standard.

**Request**

```http
GET /ap1/v1/certificate/{uln}/{familyName}/(standard}
```

Request can either use numeric "standardCode" (LARS Standard code) or "standardReference" (IFA STxxxx reference) for {standard}.

**Response** code indicates sucess or failure of the request.

Response 403, with message text 
```json
{
  "statusCode": 403,
  "message": "Cannot find apprentice with the specified Uln, FamilyName & Standard"
}
```

Response 204 if certificate can be created

Response 200 plus certificate details, if certificate already created.
  * if EPAO was not the originator of the certificate request then limited data will be provided
  * certificate may be in 'Draft', 'Ready' or 'Submitted' status
  * if in 'Draft' status then some data fields are missing

```json
{
   "certificate": {
      "certificateData": {
         "certificateReference": "string" (lookup),
         "standard": {
            "standardCode": 0 (lookup),
            "standardReference": "string" (lookup),
            "standardName": "string" (lookup),
            "level": 0 (lookup)
         },
         "learner": {
            "uln": 0 (as provided),
            "givenNames": "string" (lookup),
            "familyName": "string" (as provided)
         },
         "learningDetails": {
            "courseOption": "string" (lookup),
            "overallGrade": "string" (lookup),
            "achievementDate": "2019-02-22T00:00:00" (lookup),
            "learningStartDate": "2018-02-22T00:00:00" (lookup),
            "providerName": "string" (lookup),
            "providerUkPrn": 0 (lookup)
         },
         "postalContact": {
            "contactName": "string" (lookup),
            "department": "string" (lookup),
            "organisation": "string" (lookup),
            "addressLine1": "string" (lookup),
            "addressLine2": "string" (lookup),
            "addressLine3": "string" (lookup),
            "city": "string" (lookup),
            "postCode": "string" (lookup)
         }
      },
      "status": {
         "currentStatus": "Draft | Ready | Submitted"
      },
      "created": {
         "createdAt": "2019-02-22T11:43:20",
         "createdBy": "string"
      },
      "submitted" (if available): {
         "submittedAt": "2019-02-22T11:43:20",
         "submittedBy": "string"
      }
   }
}
```


### 2.   Create Certificate

#### To request creation of one or more certificates.

Request application/json body posted should contain an array of certificate records, for a uln, familyName and Standard combination, each with your own unique "requestId" which will be used in the response body.

Request can either use numeric "standardCode" (LARS Standard code) or "standardReference" (IFA STxxxx reference) to identify the standard.
At least one standard identifier must be provided, and if both are provided then "standardCode" and "standardReference" will be looked-up and compared.
Where the Standard has a Course Option then a valid choice must be provided.

**Request**

```http
POST /api/v1/certificate
```

application/json body posted should contain an array with the requested certificate

```json
[{
   "requestId" : "string",
   "standard": {
     "standardCode": 0 (optional)
     "standardReference": "string" (optional)
   },
   "learner": {
      "uln": 0,
      "familyName": "string"
   },
   "learningDetails": {
      "courseOption": "string" (optional),
      "overallGrade": "string",
      "achievementDate": "2018-11-16T14:26:57"
   },
   "postalContact": {
      "contactName": "string",
      "department": "string" (optional),
      "organisation": "string",
      "addressLine1": "string",
      "addressLine2": "string" (optional),
      "addressLine3": "string" (optional),
      "city": "string",
      "postCode": "string"
   }
}]
```

To request multiple certificate records in a single POST there can be multiple requests in the array, each with a distinct "requestId". 
```json  
[{"requestId": .. },{"requestId": .. },{"requestId": .. }]
```

**Response** application/json body will provide success and error responses.

Response 200, plus application/json containing response for the requested certificate, by your provided "requestId"
   * if EPAO has the correct profile to assess the requested Standard and all required data has been provided and is valid, certificate details will be returned with a status of 'Ready',
   * otherwise validation error(s) will be returned 

```json
[{
   "requestId": "string" (as provided),
   "certificate": {
      "certificateData": {
         "certificateReference": "string" (generated),
         "standard": {
            "standardCode": 0 (lookup),
            "standardReference": "string" (lookup),
            "standardName": "string" (lookup),
            "level": 0 (lookup)
         },
         "learner": {
            "uln": 0 (as provided),
            "givenNames": "string" (lookup),
            "familyName": "string" (as provided)
         },
         "learningDetails": {
            "courseOption": "string" (as provided),
            "overallGrade": "string" (as provided),
            "achievementDate": "2019-02-22T00:00:00" (as provided),
            "learningStartDate": "2018-02-22T00:00:00" (lookup),
            "providerName": "string" (lookup),
            "providerUkPrn": 0 (lookup)
         },
         "postalContact": {
            "contactName": "string" (as provided),
            "department": "string" (as provided),
            "organisation": "string" (as provided),
            "addressLine1": "string" (as provided),
            "addressLine2": "string" (as provided),
            "addressLine3": "string" (as provided),
            "city": "string" (as provided),
            "postCode": "string" (as provided),
         }
      },
      "status": {
         "currentStatus": "Draft | Ready"
      },
      "created": {
         "createdAt": "2019-02-22T11:43:20",
         "createdBy": "string"
      }
   },
   "validationErrors": []
},
{
    "requestId": "string",
    "validationErrors": ["message text","message text"]
}]
```

where "message text" is:
- Certificate 
    * "Certificate already exists: ```certificateReference```"
- Uln 
    * "The apprentice's ULN should contain exactly 10 numbers"
    * "Cannot find apprentice with the specified Uln, FamilyName & Standard" 
- Standard 
	* "A Standard should be selected"
    * "EPAO is not registered for this Standard"
	* "StandardReference and StandardCode relate to different standards*
- CourseOption
    * "Invalid course option for this Standard. Must be empty"
    * "Invalid course option for this Standard. Must be one of the following: 'list of course options'"
    where 'list of course options' depends on the standard code, and can be obtained with 
    ```http 
    GET api/v1/certificate/options/(standard}
    ```
- OverallGrade
    * "Provide the grade the apprentice achieved"
    * "Invalid grade. Must be one of the following: ```list of valid grades```", where 'list of valid grades' is: 'Pass', 'Credit', 'Merit', 'Distinction', 'Pass with excellence', 'No grade awarded' and can be obtained with
    ```http
    GET api/v1/certificate/grades
    ```
- FamilyName 
    * "Enter the apprentice's family name" 
- AchievementDate
    * "Provide the achievement date"
    * "An achievement date cannot be before 01 01 2017"
    * "An achievement date cannot be in the future"
- ContactName
    * "Enter a contact name"
- Organisation
    * "Enter an organisation"
- AddressLine1
    * "Enter an address"
- City
    * "Enter a city or town"
- Postcode
    * "Enter a postcode"
	* "Enter a valid UK postcode"


### 3.   Update Certificate (Optional)

#### To enable previously created certificates to be updated.

Previously created certificates can be updated, where they have not yet been submitted, to amend any of the data fields provided.
The certificate records to be updated have to be identified using uln, familyName, standard and the previously assigned certificateReference.

Request can either use numeric "standardCode" (LARS Standard code) or "standardReference" (IFA STxxxx reference) to identify the standard.
At least one standard identifier must be provided, and if both are provided then "standardCode" and "standardReference" will be looked-up and compared.
Where the Standard has a Course Option then a valid choice must be provided.

**Request**

```http
PUT /api/v1/certificate
```

application/json body posted should contain an array for previously requested certificates to be amended.

```json
[{
   "requestId" : "string",
   "certificateReference": "string" (as returned in  **Create Certficate POST** or using **Check Certificate GET**)
   "standard": {
     "standardCode": 0 (optional)
     "standardReference": "string" (optional)
   },
   "learner": {
      "uln": 0,
      "familyName": "string"
   },
   "learningDetails": {
      "courseOption": "string" (optional),
      "overallGrade": "string",
      "achievementDate": "2018-11-16T14:26:57.009"
   },
   "postalContact": {
      "contactName": "string",
      "department": "string" (optional),
      "organisation": "string",
      "addressLine1": "string",
      "addressLine2": "string" (optional),
      "addressLine3": "string" (optional),
      "city": "string",
      "postCode": "string"
   }
}]
```

To update multiple certificate records in a single PUT there can be multiple requests in the array, each with a distinct "requestId". 
```json  
[{"requestId": .. },{"requestId": .. },{"requestId": .. }]
```

**Response** application/json body will provide success and error responses, depending on the status of certificates.

Response 200, plus application/json containing response for the requested certificate, by your provided "requestId"
   * if EPAO has the correct profile to assess the requested Standard and all required data has been provided and is valid, certificate details will be returned with a status of 'Ready',
   * otherwise validation error(s) will be returned 

Response body is as for **Create Certificate POST**, except alternative "message text" is:
- Certificate 
    * "Enter the certificate reference"
	* "Certificate not found"
    * "EPAO is not the creator of this Certificate"
    * "Certificate is not in 'Draft' status"


### 4.   Submit Certificate

#### To submit certificates to be issued to Apprentices 

Certificates that have been created with **Create Certificate POST**, or updated **Update Certificate PUT**, should be submitted to be printed and delivered when all data has been correctly provided and checked by the EPAO.

Certificates can only be submitted after all validation checks have been successfully performed, and the certificate is 'Ready'

**Request**
  
```http
POST /api/v1/certificate/submit
```
  
The application/json body posted should contain an array of previously created certificates to be submitted, each with your own unique "requestId" which will be used in the response body
Request can either use numeric "standardCode" (LARS Standard code) or "standardReference" (IFA STxxxx reference) to identify the standard.
At least one standard identifier must be provided, and if both are provided then "standardCode" and "standardReference" will be looked-up and compared.

```json  
[{
    "requestId" : "string",
    "uln": 0,
    "standardCode": 0 (optional),
    "standardReference": "string" (optional)
    "familyName": "string",
    "certificateReference": "string"
}]
```

To submit multiple certificate records in a single POST there can be multiple requests in the array, each with a distinct "requestId". 
```json  
[{"requestId": .. },{"requestId": .. },{"requestId": .. }]
```

**Response** application/json body will provide success and error responses, depending on the status of certificates.

Response 200, plus application/json containing response for the submitted certificates, by your provided "requestId"
   * if EPAO created the certificate and status is 'Ready', certificate details will be returned and the certificate will have a status of 'Submitted',
   * otherwise validation error(s) will be returned 
   
```json
[{
   "certificate": {
      "certificateData": {
         "certificateReference": "string",
         "standard": {
            "standardCode": 0,
            "standardReference": "string",
            "standardName": "string",
            "level": 0
         },
         "learner": {
            "uln": 0,
            "givenNames": "string",
            "familyName": "string"
         },
         "learningDetails": {
            "courseOption": "string",
            "overallGrade": "string",
            "achievementDate": "2019-02-22T00:00:00",
            "learningStartDate": "2018-02-22T00:00:00",
            "providerName": "string",
            "providerUkPrn": 0
         },
         "postalContact": {
            "contactName": "string",
            "department": "string",
            "organisation": "string",
            "addressLine1": "string",
            "addressLine2": "string",
            "addressLine3": "string",
            "city": "string",
            "postCode": "string"
         }
      },
      "status": {
         "currentStatus": "Submitted"
      },
      "created": {
         "createdAt": "2019-02-22T11:43:20",
         "createdBy": "string"
      },
      "submitted": {
         "submittedAt": "2019-02-22T11:43:20",
         "submittedBy": "string"
      }
   },
   "validationErrors": []
},
{
    "requestId": "string",
    "validationErrors": ["message text","message text"]
}]
```

Response body is as for **Create Certificate POST**, except alternative "message text" is:
- Certificate 
    * "Enter the certificate reference"
	* "Certificate not found"
    * "EPAO is not the creator of this Certificate"
    * "Certificate has already been submitted"
    * "Certificate is not in 'Ready' status"
    * "Certificate is missing mandatory data"
   
  
## Delete Certificate

### To delete a certificate before it is submitted

   It is possible to delete a certificate that has been created by the EPAO using the API when the status is not 'Submitted'. 

**Request**
   
```http
DELETE /api/v1/certificate/{uln}/{familyName}/(standard}/{certificateReference}
```

**Response** depends on the certificate status, as it can only be deleted if not yet submitted.

Response 204 to confirm certificate has been deleted.

Response 403
```json
{
  "statusCode": 403,
  "message": "message text"
}
```

where "message text" is:
- Certificate 
    * "Enter the certificate reference"
	* "Certificate not found"
    * "EPAO is not the creator of this Certificate"
    * "Cannot delete a submitted Certificate"
- Uln 
    * "The apprentice's ULN should contain exactly 10 numbers"
	* "Cannot find apprentice with the specified Uln, FamilyName & Standard" 
- Standard 
    * "A Standard should be selected"
    * "EPAO is not registered for this Standard"
	* "StandardReference and StandardCode relate to different standards*
- FamilyName 
    * "Enter the apprentice's family name"


## Get Options

### To get the latest list of Course options by Standard.

The full list of options can be provided, or the list can be filtered by a standard.


**Request**

```http
GET /api/v1/certificate/options
GET /api/v1/certificate/options/(standard}
```

Request can either use numeric "standardCode" (LARS Standard code) or "standardReference" (IFA STxxxx reference) for {standard}..

**Response** application/json list of standard codes and related options.

Response 200 
```json
[{
   "standardCode": 6,
   "standardReference": "ST0156",
   "courseOption": [
      "Overhead lines",
      "Substation fitting",
      "Underground cables"
   ]
}
,
{
   "standardCode": 7,
   "standardReference": "ST0184",
   "courseOption": [
      "Card services",
      "Corporate/Commercial",
      "Retail",
      "Wealth"
   ]
}
,
... for all available standards and options
{
   "standardCode": 314,
   "standardReference": "ST0018",
   "courseOption": [
      "Container based system",
      "Soil based system"
   ]
}]
```

Response 204 - the standard was found, however it has no options

Response 404 - the standard was not found


## Get Grades 

### To get the list of valid pass grades, to use when creating certificates

**Request**

```http
GET /api/v1/certificate/grades
```

**Response** 200, plus application/json list of pass grades
```json
[
	"Pass",
	"Credit",
	"Merit",
	"Distinction",
	"Pass with excellence",
	"No grade awarded"
]
```