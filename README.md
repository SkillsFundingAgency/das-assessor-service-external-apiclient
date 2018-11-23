# ![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png) Digital Apprenticeships Service

#  End Point Assessor Certification API (das-assessor-service-external-apiclient)

![Build Status](https://sfa-gov-uk.visualstudio.com/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/831/badge)

## Developer Setup

### Requirements

- Install [Visual Studio 2017](https://www.visualstudio.com/downloads/) with these workloads:
    - ASP.NET and web development
    - .NET desktop development
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

## Sample Scenarios
For details see the online Swagger documentation in the [Developer Portal](https://developers.apprenticeships.sfa.bis.gov.uk/).

### Request a single Certificate for a Learner with a Standard Code

#### 1.   (Optional) Check if certificate exists

```http
GET /ap1/v1/certificate/{uln}/{familyName}/{standardCode}
```

returns status code

- 204 if certificate can be created
- 200 plus certificate details, if certificate already created.
  * if EPAO was not the originator of the certificate request then limited data will be provided
  * certificate may be in 'draft', 'ready' or 'submitted' status
  * if in 'draft' status then response will include validation of data fields that are missing or incorrect

#### 2.   To request a certificate when no certificate found at step 1, or skipped step 1

```http
POST /api/v1/certificate
```

application/json body posted should contain an array for the requested certificate

```json
[{
	"requestId" : "string",
	"standard": {
		"standardCode": 0
	},
	"learner": {
		"uln": 0,
		"familyName": "string"
	},
	"learningDetails": {
		"courseOption": "string",
		"overallGrade": "string",
		"achievementDate": "2018-11-16T14:26:57.009Z"
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
}]
```

returns status code
- 200 plus application/json containing response for the requested certificate, by your provided "requestId"
   * if EPAO has the correct profile to assess the requested Standard and all required data has been provided and is valid, certificate details will be returned with a status of 'ready',
   * otherwise validation error(s) will be returned 


#### 3.   To submit a certificate created at step 2
Certificate can only be submitted after all validation checks have been performed, and the certificate is 'ready'
  
```http
POST /api/v1/certificate/submit
```
  
application/json body posted should contain an array of submitted certificates, each with your own unique "requestId" which will be used in the response body

```json  
[{
	"requestId" : "string",
	"uln": 0,
	"standardCode": 0,
	"familyName": "string",
	"certificateReference": "string"
}]
```

returns status code
- 200 plus application/json containing response for the submitted certificate, by your provided "requestId"
   * if EPAO created the certificate and status is 'ready', certificate details will be returned and the certificate will have a status of 'submitted',
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
   * if EPAO has the correct profile to assess the requested Standard and all required data has been provided and is valid, certificate details will be returned each certificate will have a status of 'ready',
   * otherwise validation error(s) will be returned for each certificate failing validation

#### 2.   To submit a certificates created at step 1
Certificates can only be submitted after all validation checks have been performed, and the certificate is 'ready'.
  
```http
POST /api/v1/certificate/submit
```

application/json body posted should contain an array of submitted certificates, each with your own unique "requestId" which will be used in the response body

```json  
[{"requestId": .. },{"requestId": .. },{"requestId": .. }]
```

returns status code
- 200 plus application/json response for each requested certificate, by your provided "requestId"
   * if EPAO created the certificate and status is 'ready', certificate details will be returned and the certificate will have a status of 'submitted',
   * otherwise validation error(s) will be returned 
   
### Delete a Certificate

#### To delete a certificate before it is submitted

   It is possible to delete a certificate that has been created by the EPAO using the API when the status is not 'submitted'. 

```http
DELETE /api/v1/certificate/{uln}/{familyName}/{standardCode}/{certificateReference}
```

returns status code
- 204 to confirm certificate has been deleted.
 


## License
Licensed under the [MIT license](https://github.com/SkillsFundingAgency/das-assessor-service-external-apiclient/blob/master/LICENSE)