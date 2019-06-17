# ![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png) Digital Apprenticeships Service

#  End Point Assessor Certification API (das-assessor-service-external-apiclient)

![Build Status](https://sfa-gov-uk.visualstudio.com/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/831/badge)

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

### Request a single Certificate for a Learner with a Standard

#### 1.   (Optional) Check if certificate exists

```http
GET /ap1/v1/certificate/{uln}/{familyName}/{standard}
```

returns status code
- 204 if certificate can be created
- 200 plus certificate details, if certificate already created.
  * if EPAO was not the originator of the certificate request then limited data will be provided
  * certificate may be in 'Draft', 'Ready' or 'Submitted' status
  * if in 'Draft' status then response will include validation of data fields that are missing or incorrect
- 403 validation error occurred

#### 2.   To request a certificate when no certificate found at step 1, or skipped step 1

```http
POST /api/v1/certificate
```

application/json body posted should contain an array for the requested certificate

```json
[{
	"requestId" : "string",
	"standard": {
		"standardCode": 0 (optional if standardReference specified),
		"standardReference": "string" (optional if standardCode specified)
	},
	"learner": {
		"uln": 0,
		"familyName": "string"
	},
	"learningDetails": {
		"courseOption": "string",
		"overallGrade": "Pass | Credit | Merit | Distinction | Pass with excellence | No grade awarded",
		"achievementDate": "2018-11-16T14:26:57.009Z"
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

returns status code
- 200 plus application/json containing response for the requested certificate, by your provided "requestId"
   * if EPAO has the correct profile to assess the requested Standard and all required data has been provided and is valid, certificate details will be returned with a status of 'Ready',
   * otherwise validation error(s) will be returned


#### 3.   To submit a certificate created at step 2
Certificate can only be submitted after all validation checks have been performed, and the certificate is 'Ready'
  
```http
POST /api/v1/certificate/submit
```
  
application/json body posted should contain an array for the submitted certificate

```json  
[{
	"requestId" : "string",
	"uln": 0,
	"standardCode": 0 (optional if standardReference specified),
	"standardReference": "string" (optional if standardCode specified),
	"familyName": "string",
	"certificateReference": "string"
}]
```

returns status code
- 200 plus application/json containing response for the submitted certificate, by your provided "requestId"
   * if EPAO created the certificate and status is 'Ready', certificate details will be returned and the certificate will have a status of 'Submitted',
   * otherwise validation error(s) will be returned 
   

### Request a Batch of Certificates

#### 1.   To request multiple certificates

```http
POST /api/v1/certificate
```

application/json body posted should contain an array of certificate requests, each with your own unique "requestId" which will be used in the response body

```json
[{"requestId": .. },{"requestId": .. },{"requestId": .. }]
```
returns status code
- 200 plus application/json response for each requested certificate, by your provided "requestId"
   * if EPAO has the correct profile to assess the requested Standard and all required data has been provided and is valid, certificate details will be returned each certificate will have a status of 'Ready',
   * otherwise validation error(s) will be returned for each certificate failing validation
- 403 Too many certificates specified within the request.
   * current limit is 25

#### 2.   To submit a certificates created at step 1
Certificates can only be submitted after all validation checks have been performed, and the certificate is 'Ready'.
  
```http
POST /api/v1/certificate/submit
```

application/json body posted should contain an array of submitted certificates, each with your own unique "requestId" which will be used in the response body

```json  
[{"requestId": .. },{"requestId": .. },{"requestId": .. }]
```

returns status code
- 200 plus application/json response for each requested certificate, by your provided "requestId"
   * if EPAO created the certificate and status is 'Ready', certificate details will be returned and the certificate will have a status of 'Submitted',
   * otherwise validation error(s) will be returned
- 403 Too many certificates specified within the request.
   * current limit is 25   
   
### Delete a Certificate

#### To delete a certificate before it is submitted

   It is possible to delete a certificate that has been created by the EPAO using the API when the status is not 'Submitted'. 

```http
DELETE /api/v1/certificate/{uln}/{familyName}/{standard}/{certificateReference}
```

returns status code
- 204 to confirm certificate has been deleted.
 
### Request the list of valid Certificate Grades

#### 1.   To request grades

```http
GET /ap1/v1/certificate/grades
```

returns status code
- 200 plus the list of valid certificate grades.

### Request the list of Options for a Standard

#### 1.   To request options

```http
GET /ap1/v1/standards/options/{standard}
```

returns status code
- 200 plus the list of valid options for the standard.
- 204 standard found, however it has no options
- 404 the standard was not found


## License
Licensed under the [MIT license](https://github.com/SkillsFundingAgency/das-assessor-service-external-apiclient/blob/master/LICENSE)