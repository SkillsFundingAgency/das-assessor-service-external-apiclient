# ![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png) Digital Apprenticeships Service

#  End Point Assessor Certification API (das-assessor-service-external-apiclient)

![Build Status](https://sfa-gov-uk.visualstudio.com/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/831/badge)

## License

Licensed under the [MIT license](https://github.com/SkillsFundingAgency/das-assessor-service-external-apiclient/blob/master/LICENSE)

## Developer Setup

### Requirements

- Install [Visual Studio 2017 or 2019](https://www.visualstudio.com/downloads/) with these workloads:
    - ASP.NET and web development
    - .NET desktop development
    - .NET Core 2.1 SDK
- Create an account on the [Developer Portal](https://developers.apprenticeships.education.gov.uk/)
    - Obtain External API Subscription Key and Base Address to Sandbox Environment
    - Can also be used to access the current Swagger Documentation

### Open the solution

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
For details see the online Swagger documentation in the [Developer Portal](https://developers.apprenticeships.education.gov.uk/).

## Monthly Sandbox ILR Test data refresh

On the 1st of every month the Sandbox Environment will be refreshed with a test data set based on the current [Register of End Point Assessor Organisations.](https://www.gov.uk/guidance/register-of-end-point-assessment-organisations)

The set of ILR records follow this pattern:

##### uln = 10 digits  
- "1" - leading digit
- "xxxx" - 4 digits of EPAOrgId
- "xxx" - 3 digits of LARS Standard Code (leading 0s)
- "00 - 09" - 10 unique ulns per standard code

***Example. For EPA0001, Standard Code = 80, and 1st uln in the sequence***
- uln = "1" + "0001" + "080" + "01" = 1000108001
- givenNames = Test
- familyName = 1000108001 (same value as uln)
- standard = 80

### Standard versions
Versions of standards supported are as published by IfATE [Standards](https://www.instituteforapprenticeships.org/apprenticeship-standards)
The explicit use of standard version on creation of EPA records and certificates is now expected. If supplied the "version" field value will be taken from the request application/json data object, and validated against the IfATE standard data, and the EPAO's approved versions of the standard. If the "version" field is not supplied a default value will be calculated, based on the apprenticeship start date and the version of the standard active at that time. Note, that this may not always be accurate and hence it is recommended that the "version" should be provided.

## Learner Details

### GET Learner Details

#### To get details held by the Assessor Service for a Learner

**Request**

```http
GET /ap1/v1/learner/{uln}/{familyName}/{standard}
```

Request can either use numeric "standardCode" (LARS Standard code) or "standardReference" (IfATE STxxxx reference) for \{standard}.

**Response** depends on whether the learner can be verified against current ILR and Apprenticeship records in the Assessor Service,
there is an existing EPA record or a certificate has already been requested. If already held in the Apprenticeship service records for the learner, then "version" will be provided in the "standard" response. 

Response 403, with message text 
```json
{
  "statusCode": 403,
  "message": "Cannot find apprentice with the specified Uln, FamilyName & Standard"
}
```

Response 200, with application/json body dependent on records held.

**Examples**

***A. No EPA Record yet created, learner is known to the Assessor Service***

```json
{
    "learnerData": {
        "standard": {
            "standardCode": 0 (as provided/lookup),
            "standardReference": "string" (as provided/lookup),
            "standardName": "string" (lookup),
            "level": 0 (lookup)
        },
        "learner": {
            "uln": 0 (as provided),
            "givenNames": "string" (lookup),
            "familyName": "string" (as provided)
        },
        "learningDetails": {
            "learnerReferenceNumber": "string" (lookup),
            "learningStartDate": "2018-02-22T00:00:00" (lookup),
            "plannedEndDate": "2019-02-22T00:00:00" (lookup),
            "providerName": "string" (lookup),
            "providerUkPrn": 0 (lookup),
            "version": "string" (lookup, where already held),
            "courseOption": "string" (lookup, where already held)
},
    },
    "status": {
        "completionStatus": "string" (lookup)
    }
}
```

***B. EPA Record found for learner, there is no associated certificate***

```json
{
    "learnerData": {
        "standard": {
            "standardCode": 0 (as provided/lookup),
            "standardReference": "string" (as provided/lookup),
            "standardName": "string" (lookup),
            "level": 0 (lookup)
        },
        "learner": {
            "uln": 0 (as provided),
            "givenNames": "string" (lookup),
            "familyName": "string" (as provided)
        },
        "learningDetails": {
            "learnerReferenceNumber": "string" (lookup),
            "learningStartDate": "2018-02-22T00:00:00" (lookup),
            "plannedEndDate": "2019-02-22T00:00:00" (lookup),
            "providerName": "string" (lookup),
            "providerUkPrn": 0 (lookup),
            "version": "string" (lookup, where already held),
            "courseOption": "string" (lookup, where already held)
        },
    },
    "status": {
        "completionStatus": "string" (lookup)
    },
    "epaDetails": {
        "epaReference": "string" (lookup),
        "epas": [{
                    "epaDate": "2019-02-02T00:00:00Z",
                    "epaOutcome": "pass | fail | withdrawn",
                },
                {
                    "epaDate": "2019-02-16T00:00:00Z",
                    "epaOutcome": "pass | fail | withdrawn",
                    "resit": true | false,
                    "retake": true | false
                }],
        "latestEPADate": "2019-02-16T00:00:00Z" (lookup),
        "latestEPAOutcome": "pass | fail | withdrawn" (lookup)
    }
}
```

***C. Certificate found for learner, with/without EPA Record***

```json
{
    "learnerData": {
        "standard": {
            "standardCode": 0 (as provided/lookup),
            "standardReference": "string" (as provided/lookup),
            "standardName": "string" (lookup),
            "level": 0 (lookup)
        },
        "learner": {
            "uln": 0 (as provided),
            "givenNames": "string" (lookup),
            "familyName": "string" (as provided)
        },
        "learningDetails": {
            "learnerReferenceNumber": "string" (lookup),
            "learningStartDate": "2018-02-22T00:00:00" (lookup),
            "plannedEndDate": "2019-02-22T00:00:00" (lookup),
            "providerName": "string" (lookup),
            "providerUkPrn": 0 (lookup),
            "version": "string" (lookup),
            "courseOption": "string" (lookup, optional)
        },
    },
    "status": {
        "completionStatus": "string" (lookup)
    },
    "epaDetails": {
        "epaReference": "string" (lookup),
        "epas": [{
                    "epaDate": "2019-02-02T00:00:00Z",
                    "epaOutcome": "pass | fail | withdrawn",
                },
                {
                    "epaDate": "2019-02-12T00:00:00Z",
                    "epaOutcome": "pass | fail | withdrawn",
                    "resit": true | false,
                    "retake": true | false
                }],
        "latestEPADate": "2019-02-12T00:00:00Z" (lookup),
        "latestEPAOutcome": "pass | fail | withdrawn" (lookup)
    },
   "certificate": {
      "certificateData": {
         "certificateReference": "string" (lookup),
         "standard": {
            "standardCode": 0 (as provided/ookup),
            "standardReference": "string" (as provided/lookup),
            "standardName": "string" (lookup),
            "level": 0 (lookup)
         },
         "learner": {
            "uln": 0 (as provided),
            "givenNames": "string" (lookup),
            "familyName": "string" (as provided)
         },
         "learningDetails": {
            "version": "string" (lookup),
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



## Record Assessment Outcomes

### 1.   Create EPA Record

#### To create one or more EPA records, including all historical EPA attempts and outcomes

Request application/json body posted should contain an array of EPA records, each with your own unique "requestId", which will be used in the response body.

Request body can either use numeric "standardCode" (LARS Standard code) or "standardReference" (IfATE STxxxx reference) to identify the standard. At least one standard identifier must be provided, and if both are provided then "standardCode" and "standardReference" will be looked-up and compared. The "learningDetails" data object, comprising "version" and "courseOption" fields, is optional but should be provided if known, where "version" relates to the available IfATE versions for the "standardReference", and will be validated against available versions of the standard, and the EPAO's approved versions for the standard. If "version" is not provided a default value will be calculated based on the apprenticeship start date held by the Assessor Service. When provided "courseOption" will be validated against possible Course options for the standard version. 

**Request**

```http
POST /api/v1/epa
```

application/json body posted should contain an array with the requested EPA records

```json
[{
   "requestId" : "string",
   "standard": {
      "standardCode": 0 (optional),
      "standardReference": "string" (optional)
   },
   "learner": {
      "uln": 0,
      "familyName": "string"
   },
   "learningDetails": {
      "version": "string" (optional),
      "courseOption": "string" (optional)
   } (optional),   
   "epaDetails": {
        "epas": [{
                "epaDate": "2019-02-02T00:00:00Z",
                "epaOutcome": "pass | fail | withdrawn"
            }]
   }
}]
```

It is expected that the first EPA record created for an apprenticeship will normally contain a single EPA outcome. When initially creating EPA records on the Assessor Service via the API for historical records, it is possible that a learner may have already had multiple EPAs, in which case all of these should be provided in the initial **Create EPA Record POST**. See **Update EPA Record PUT** to handle further EPA outcomes for a learner.

To request multiple EPA records in a single POST there can be multiple requests in the array, each with a distinct "requestId". 
```json  
[{"requestId": .. },{"requestId": .. },{"requestId": .. }]
```

The maximum POST size is limited to 32k bytes.
This is approximately 25 EPA Record requests in each API call and is capped at that limit.

**Response** application/json body will provide success and error responses.

Response 200, plus application/json containing response for the requested EPA record, by your provided "requestId"
   * EPAO has the correct profile to assess the requested Standard and all required data has been provided and is valid, EPA reference will be returned
   * otherwise validation error(s) will be returned 

```json
[{
   "requestId": "string" (as provided),
   "epaReference": "string" (generated),
   "validationErrors": []
},
{
    "requestId": "string" (as provided),
    "validationErrors": ["message text", "message text"]
}]
```

Where "message text" is:
- EPA data 
    * "Certificate already exists, cannot create EPA record"
    * "EPA already provided for the learner"
    * "Invalid outcome: must be pass, fail or withdrawn"
    * "EPA Date cannot be in the future"
- Uln 
    * "ULN should contain exactly 10 numbers"
    * "ULN, FamilyName and Standard not found" 
- Standard 
    * "Provide a valid Standard"
    * "Invalid version for Standard"
    * "Your organisation is not approved to assess this Standard"
    * "Your organisation is not approved to assess this Standard version"
    * "StandardReference and StandardCode must be for the same Standard"
- CourseOption
    * "No course option available for this Standard and version. Must be empty"
    * "Invalid course option for this Standard and version. Must be one of the following: 'list of course options'"
    where 'list of course options' depends on the standard code, and can be obtained with 
    ```http 
    GET /api/v1/standards/options/{standard}/{version}
    ```
- FamilyName 
    * "Provide apprentice family name" 



### 2.   Update EPA Record (Optional)

#### To update a previously recorded assessment outcome.

Use this to update one or more EPA records, for all outcomes to date.

Request application/json body posted should contain an array of EPA records, each with your own unique "requestId" which will be used in the response body.
The latest and all previous EPA outcomes for the learner and standard combination should be included for the update.
If the first EPA outcome is a fail and there is subsequently a pass the EPA details can include an optional "resit" or "retake" boolean.
If "resit" or "retake" are not included the value will be false by default. If an earlier EPA was for a different version of the standard the "version" must be provided, otherwise the version will be taken from the earlier EPA record held by the Assessor Service.

Request can either use numeric "standardCode" (LARS Standard code) or "standardReference" (IfATE STxxxx reference) to identify the standard.
At least one standard identifier must be provided, and if both are provided then "standardCode" and "standardReference" will be looked-up and compared. The "learningDetails" data object, comprising "version" and "courseOption" fields, is optional but should be provided if known, where  "version" field relates to the available IfATE versions for the "standardReference", and will be validated against available versions of the standard if provided, and the EPAO's approved versions for the standard. If "version" is not provided the value will be taken from the initial **Create EPA Record POST**. When provided "courseOption" will be validated against possible Course options for the standard version. 


The request should use the previously returned EPA Reference (which can also be obtained using GET Learner Details)

**Request**

```http
PUT /api/v1/epa
```

application/json body posted should contain an array for previously requested certificates to be amended.

```json
[{
   "requestId" : "string",
   "epaReference" : "string" (as returned in  **Create EPA Record POST** or using **Get Learner Details GET**),
   "standard": {
      "standardCode": 0 (optional),
      "standardReference": "string" (optional)
   },
   "learner": {
      "uln": 0,
      "familyName": "string"
   },
   "learningDetails": {
      "version": "string" (optional),
      "courseOption": "string" (optional)
   } (optional),
   "epaDetails": {
        "epas": [{
                "epaDate": "2019-01-02T00:00:00Z",
                "epaOutcome": "pass | fail | withdrawn"
            }, {
                "epaDate": "2019-02-12T00:00:00Z",
                "epaOutcome": "pass | fail | withdrawn",
                "resit": true | false (optional),
                "retake": true | false (optional)
            }
        ]
    }
}]
```

To update multiple EPA records in a single POST there can be multiple requests in the array, each with a distinct "requestId". 
```json  
[{"requestId": .. },{"requestId": .. },{"requestId": .. }]
```

The maximum PUT size is limited to 32k bytes.
This is approximately 25 EPA Record requests in each API call and is capped at that limit.

**Response** application/json body will provide success and error responses.

Response 200, plus application/json containing response for the requested EPA record, by your provided "requestId"
   * EPAO has the correct profile to assess the requested Standard and all required data has been provided and is valid, EPA Reference will be returned
   * otherwise validation error(s) will be returned 

```json
[{
    "requestId": "string" (as provided),
    "epaReference": "string" (as provided),
    "validationErrors": ["message text", "message text"]
}]
```

Where "message text" is:
- EPA data 
    * "Certificate already exists, cannot create EPA record"
    * "EPA not found"
    * "Your organisation is not the creator of this EPA"
    * "Provide the EPA reference"
    * "Invalid outcome: must be pass, fail or withdrawn"
    * "EPA Date cannot be in the future"
- Uln 
    * "ULN should contain exactly 10 numbers"
    * "ULN, FamilyName and Standard not found" 
- Standard 
    * "Provide a valid Standard"
    * "Invalid version for Standard"
    * "Your organisation is not approved to assess this Standard"
    * "Your organisation is not approved to assess this Standard version"
    * "StandardReference and StandardCode must be for the same Standard"
- CourseOption
    * "No course option available for this Standard and version. Must be empty"
    * "Invalid course option for this Standard and version. Must be one of the following: 'list of course options'"
    where 'list of course options' depends on the standard code, and can be obtained with 
    ```http 
    GET /api/v1/standards/options/{standard}/{version}
    ```
- FamilyName 
    * "Provide apprentice family name" 


## Delete EPA Record

### To delete a previously recorded assessment outcome

   It is possible to delete the EPA Record that has been created by the EPAO using the API when a certificate has not been requested for the learner. 

**Request**
   
```http
DELETE /api/v1/epa/{uln}/{familyName}/{standard}/{epaReference}
```

**Response** code indicates success or failure of the request.

Response 204 to confirm EPA record has been deleted.

Response 403
```json
{
  "statusCode": 403,
  "message": "message text"
}
```

Where "message text" is:
- EPA data 
    * "Provide the EPA reference"
    * "EPA not found"
    * "Your organisation is not the creator of this EPA"
    * "Certificate already exists, cannot delete EPA record"
- Uln 
    * "ULN should contain exactly 10 numbers"
    * "ULN, FamilyName and Standard not found" 
- Standard 
    * "Provide a valid Standard"
    * "Your organisation is not approved to assess this Standard"
- FamilyName 
    * "Provide apprentice family name"


## Request Certificate for Apprenticeships 

### 1.   Check Certificate (Optional) 

#### To check on the status of a learner's certificate for a specific standard.

**Request**

```http
GET /ap1/v1/certificate/{uln}/{familyName}/{standard}
```

Request can either use numeric "standardCode" (LARS Standard code) or "standardReference" (IfATE STxxxx reference) for \{standard}.

**Response** code indicates success or failure of the request.

Response 403, with message text 
```json
{
  "statusCode": 403,
  "message": "Cannot find [ apprentice | certificate ] with the specified Uln, FamilyName & Standard"
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
            "standardCode": 0 (as provided/lookup),
            "standardReference": "string" (as provided/lookup),
            "standardName": "string" (lookup),
            "level": 0 (lookup)
         },
         "learner": {
            "uln": 0 (as provided),
            "givenNames": "string" (lookup),
            "familyName": "string" (as provided)
         },
         "learningDetails": {
            "version": "string" (lookup),
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

Request can either use numeric "standardCode" (LARS Standard code) or "standardReference" (IfATE STxxxx reference) to identify the standard.
At least one standard identifier must be provided, and if both are provided then "standardCode" and "standardReference" will be looked-up and compared. The "version" field is optional but should be provided where known, and relates to the available IfATE versions for the "standardReference", and will be validated against available versions of the standard, and the EPAO's approved versions for the standard. If "version" is not provided the value will be taken from a previous **Create EPA Record POST** (if used) or a default value will be calculated based on the apprenticeship start date held by the Assessor Service.

Where the Standard has a Course Option then a valid choice must be provided, which is valid for the specific standard version.

**Request**

```http
POST /api/v1/certificate
```

application/json body posted should contain an array with the requested certificate

```json
[{
   "requestId" : "string",
   "standard": {
      "standardCode": 0 (optional),
      "standardReference": "string" (optional)
   },
   "learner": {
      "uln": 0,
      "familyName": "string"
   },
   "learningDetails": {
      "version": "string" (optional),
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

The maximum POST size is limited to 32k bytes.
This is approximately 25 Certificate requests in each API call and is capped at that limit.

**Response** application/json body will provide success and error responses.

Response 200, plus application/json containing response for the requested certificate, by your provided "requestId"
   * EPAO has the correct profile to assess the requested Standard and all required data has been provided and is valid, certificate details will be returned with a status of 'Ready'
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
            "version": "string" (lookup),
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
    "validationErrors": ["message text", "message text"]
}]
```

Where "message text" is:
- Certificate 
    * "Certificate already exists: ```certificateReference```"
- Uln 
    * "ULN should contain exactly 10 numbers"
    * "ULN, FamilyName and Standard not found" 
- Standard 
    * "Provide a valid Standard"
    * "Invalid version for Standard"
    * "Your organisation is not approved to assess this Standard"
    * "Your organisation is not approved to assess this Standard version"
    * "StandardReference and StandardCode must be for the same Standard*
- CourseOption
    * "No course option available for this Standard and version. Must be empty"
    * "Invalid course option for this Standard and version. Must be one of the following: 'list of course options'"
    where 'list of course options' depends on the standard code, and can be obtained with 
    ```http 
    GET /api/v1/standards/options/{standard}/{version}
    ```
- OverallGrade
    * "Select the grade the apprentice achieved"
    * "You must enter a valid grade. Must be one of the following: ```list of valid grades```", where 'list of valid grades' is: 'Pass', 'Credit', 'Merit', 'Distinction', 'Pass with excellence', 'No grade awarded' and can be obtained with
    ```http
    GET api/v1/certificate/grades
    ```
- FamilyName 
    * "Provide apprentice family name" 
- AchievementDate
    * "Provide the achievement date"
    * "Achievement date cannot be before 01 01 2017"
    * "Achievement date cannot be in the future"
- ContactName
    * "Provide a contact name"
- Organisation
    * "Provide an organisation"
- AddressLine1
    * "Provide an address"
- City
    * "Provide a city or town"
- Postcode
    * "Provide a postcode"
    * "Provide a valid UK postcode"


### 3.   Update Certificate (Optional)

#### To enable previously created certificates to be updated.

Previously created certificates can be updated, where they have not yet been submitted, to amend any of the data fields provided.
The certificate records to be updated have to be identified using uln, familyName, standard and the previously assigned certificateReference.

Request can either use numeric "standardCode" (LARS Standard code) or "standardReference" (IfATE STxxxx reference) to identify the standard.
At least one standard identifier must be provided, and if both are provided then "standardCode" and "standardReference" will be looked-up and compared. The "version" field is optional but should be provided where known, and relates to the available IfATE versions for the "standardReference", and will be validated against available versions of the standard, and the EPAO's approved versions for the standard. If "version" is not provided the value will be taken from a previous **Create Certificate POST**

Where the Standard has a Course Option then a valid choice must be provided, which is valid for the specific standard version.

**Request**

```http
PUT /api/v1/certificate
```

application/json body posted should contain an array for previously requested certificates to be amended.

```json
[{
   "requestId" : "string",
   "certificateReference": "string" (as returned in  **Create Certificate POST** or using **Check Certificate GET**),
   "standard": {
      "standardCode": 0 (optional),
      "standardReference": "string" (optional)
   },
   "learner": {
      "uln": 0,
      "familyName": "string"
   },
   "learningDetails": {
      "version": "string" (optional),
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

The maximum PUT size is limited to 32k bytes.
This is approximately 25 Certificate requests in each API call and is capped at that limit.

**Response** application/json body will provide success and error responses, depending on the status of certificates.

Response 200, plus application/json containing response for the requested certificate, by your provided "requestId"
   * EPAO has the correct profile to assess the requested Standard and all required data has been provided and is valid, certificate details will be returned with a status of 'Ready'
   * otherwise validation error(s) will be returned 

Response body is as for **Create Certificate POST**, except alternative "message text" is:
- Certificate 
    * "Provide the certificate reference"
    * "Certificate not found"
    * "Your organisation is not the creator of this Certificate"
    * "Certificate does not exist in Draft status"


### 4.   Submit Certificate

#### To submit certificates to be issued to Apprentices 

Certificates that have been created with **Create Certificate POST**, or updated **Update Certificate PUT**, should be submitted to be printed and delivered when all data has been correctly provided and checked by the EPAO.

Certificates can only be submitted after all validation checks have been successfully performed, and the certificate is 'Ready'

**Request**
  
```http
POST /api/v1/certificate/submit
```
  
The application/json body posted should contain an array of previously created certificates to be submitted, each with your own unique "requestId" which will be used in the response body
Request can either use numeric "standardCode" (LARS Standard code) or "standardReference" (IfATE STxxxx reference) to identify the standard.
At least one standard identifier must be provided, and if both are provided then "standardCode" and "standardReference" will be looked-up and compared. 

```json  
[{
    "requestId" : "string",
    "uln": 0,
    "standardCode": 0 (optional),
    "standardReference": "string" (optional),
    "familyName": "string",
    "certificateReference": "string"
}]
```

To submit multiple certificate records in a single POST there can be multiple requests in the array, each with a distinct "requestId". 
```json  
[{"requestId": .. },{"requestId": .. },{"requestId": .. }]
```

The maximum POST size is limited to 32k bytes.
This is approximately 25 Submit Certificate requests in each API call and is capped at that limit.

**Response** application/json body will provide success and error responses, depending on the status of certificates.

Response 200, plus application/json containing response for the submitted certificates, by your provided "requestId"
   * EPAO created the certificate and status is 'Ready', certificate details will be returned and the certificate will have a status of 'Submitted',
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
            "version": "string",
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
    "validationErrors": ["message text", "message text"]
}]
```

Response body is as for **Create Certificate POST**, except alternative "message text" is:
- Certificate 
    * "Provide the certificate reference"
    * "Certificate not found"
    * "Your organisation is not the creator of this Certificate"
    * "Certificate has already been Submitted"
    * "Certificate is not in Ready status"
    * "Certificate is missing mandatory data"
   
  
## Delete Certificate

### To delete a certificate before it is submitted

   It is possible to delete a certificate that has been created by the EPAO using the API when the status is not 'Submitted'. 

**Request**
   
```http
DELETE /api/v1/certificate/{uln}/{familyName}/{standard}/{certificateReference}
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

Where "message text" is:
- Certificate 
    * "Provide the certificate reference"
    * "Certificate not found"
    * "Your organisation is not the creator of this Certificate"
    * "Cannot delete a Submitted Certificate"
- Uln 
    * "ULN should contain exactly 10 numbers"
    * "ULN, FamilyName and Standard not found" 
- Standard 
    * "Provide a valid Standard"
    * "Your organisation is not approved to assess this Standard"
- FamilyName 
    * "Provide apprentice family name"


## Get Options

### To get the latest list of Course options by Standard.

The full list of options can be provided, or the list can be filtered by a standard.


**Request**

```http
GET /api/v1/standards/options
GET /api/v1/standards/options/{standard}

```

Request can either use numeric "standardCode" (LARS Standard code) or "standardReference" (IfATE STxxxx reference) for \{standard}. Where Options are available, these will be for the most recent version of a standard.

**Response** application/json list of standard codes and related options.

Response 200 
```json
[{
   "standardCode": 6,
   "standardReference": "ST0156",
   "version": "1.1",   
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
   "version": "1.0",
   "courseOption": [
      "Card services",
      "Corporate/Commercial",
      "Retail",
      "Wealth"
   ]
}
,
... for all available active standards and options
{
   "standardCode": 314,
   "standardReference": "ST0018",
   "version": "1.0",   
   "courseOption": [
      "Container Based System",
      "Soil Based System"
   ]
}]
```

Response 204 - the standard was found, however it has no options

Response 404 - the standard was not found
The full list of options can be provided, or the list can be filtered by a standard.


**Request**

```http
GET /api/v1/standards/options/{standard}/{version}

```

Request can either use numeric "standardCode" (LARS Standard code) or "standardReference" (IfATE STxxxx reference) for \{standard}. The optional "version" parameter will return options related to the specific version of the Standard. 

**Response** application/json list of standard codes and related options.

Response 200 (example for **ST0156** version **1.0**)
```json
[{
   "standardCode": 6,
   "standardReference": "ST0156",
   "version": "1.0"
   "courseOption": [
      "Overhead lines",
      "Substation fitting",
      "Underground cables"
   ]
}]
```

Response 204 - the standard (or standard version) was found, however it has no options

Response 404 - the standard (or standard version) was not found


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