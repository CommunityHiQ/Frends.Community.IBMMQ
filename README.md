# Frends.Community.IBMMQ
Frends task for operating on IMBMQ queues. Supports reading and writing messages from/to a queue and getting some metadata of queues.

- [Installing](#installing)
- [Tasks](#tasks)
  - [Get Queue Status](#GetQueueStatus)
  - [Get Messages](#GetMessages)
  - [Peek Message](#PeekMessage)
  - [Put Message](#PutMessage)
- [License](#license)
- [Building](#building)
- [Contributing](#contributing)
- [Change Log](#change-log)

# Installing
You can install the task via FRENDS UI Task View or you can find the nuget package from the following nuget feed
'TODO'

# Tasks

## GetQueueStatus

Get queue status information from desired queues. Can be used for example to get the message count of a queue.

### Input

| Property             | Type                 | Description                          | Example      |
| ---------------------| ---------------------| ------------------------------------ | -----        |
| Queues               | string[]             | Queue names to get status from       | DEV.QUEUE.1  |

### Connection

| Property             | Type                 | Description                          | Example          |
| ---------------------| ---------------------| -------------------------------------| -----            |
| HostName             | string               | Host name of the IBM MQ instance     | localhost        |
| PortNumber           | int                  | Port number of the IBM MQ instance   | 1414             |
| Channel              | string               | Name of the channel                  | SAMPLE.CHANNEL   |
| QueueManagerName     | string               | Name of the Queue manager            | QM1              |
| UserID               | string               | UserID to be used for authentication & authorization (optional) | app |
| Password             | string               | Password of the UserID above (optional) | ****** |

### Options

| Property             | Type                 | Description                          | Example  |
| ---------------------| ---------------------| ------------------------------------ | -----    |
| SslCertStore         | string               | Cert store to use                    | MyStore  |
| SslCipherSpec        | string               | Cipher spec to use                   |      |
| SslPeerName          | string               | Peer name to use                     |      |
| SslResetCount        | int                  | Reset count                          | 0        |
| SslCertRevocationCheck | bool               | Cert revocation check performed      | No       |

### Returns

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | -----   |
| List                 | List&lt;GetQueueStatusOutput&gt; | A list of GetQueueStatusOutput-objects | |

#### GetQueueStatusOutput

| Property             | Type                 | Description                          | Example      |
| ---------------------| ---------------------| ------------------------------------ | -----        |
| Name                 | string               | Queue name                           | DEV.QUEUE.1  |
| MessageCount         | int                  | Number of messages in the queue      | 53           |
| Description          | string               | Queue description                    | Dev queue    |
| OldestMessageQueueTime | DateTime           | The put timestamp of the message in front of the queue (local time with timezone offset)| 2022-01-03T15:32:33.23+03:00  |

## GetMessages

Get messages from a queue.

### Input

| Property             | Type                 | Description                          | Example      |
| ---------------------| ---------------------| ------------------------------------ | -----        |
| Queue                | string               | Queue name to get messages from      | DEV.QUEUE.1  |
| MessageCount         | int                  | Max number of messages to get        | 10           |
| MessageAsBytes       | bool                 | If set, return messsage as bytes, message in MessageBytes-field | No |
| Get Message Properties | bool               | If set, return properties in MessageProperties-field | Yes |
| Get Message Descriptor | bool               | If set, return properties in MessageDescriptor-field | No |
| Parse RFH2 Header    | bool                 | If set, parse and return RHF2 headers in RFH2Headers-field. Field is null if RFH2 headers was not found. If RFH2 Headers are parsed and returned, they are not visible in the actual Message returned. | No |
| UseTransaction       | bool                 | Whether transaction is used          | Yes  |
| TransactionType      | enum                 | Transcation type (Required, RequiresNew), see details from Microsoft docs: <https://docs.microsoft.com/en-us/dotnet/framework/data/transactions/implementing-an-implicit-transaction-using-transaction-scope#ManageTxFlow>       | Required |
| TransactionTimeout   | int                  | Transction timeout in seconds        | 60  |

### Connection

| Property             | Type                 | Description                          | Example          |
| ---------------------| ---------------------| -------------------------------------| -----            |
| HostName             | string               | Host name of the IBM MQ instance     | localhost        |
| PortNumber           | int                  | Port number of the IBM MQ instance   | 1414             |
| Channel              | string               | Name of the channel                  | SAMPLE.CHANNEL   |
| QueueManagerName     | string               | Name of the Queue manager            | QM1              |
| UserID               | string               | UserID to be used for authentication & authorization (optional) | app |
| Password             | string               | Password of the UserID above (optional) | ****** |

### Options

| Property             | Type                 | Description                          | Example  |
| ---------------------| ---------------------| ------------------------------------ | -----    |
| SslCertStore         | string               | Cert store to use                    | MyStore  |
| SslCipherSpec        | string               | Cipher spec to use                   | |
| SslPeerName          | string               | Peer name to use                     | |
| SslResetCount        | int                  | Reset count                          | 0        |
| SslCertRevocationCheck | bool               | Cert revocation check performed      | No       |

### Returns

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | -----   |
| Data                 | List&lt;QueueMessage&gt; | A list of QueueMessage-objects | |

#### QueueMessage

| Property             | Type                 | Description                          | Example  |
| ---------------------| ---------------------| ------------------------------------ | ---------|
| Message              | string               | Message contents if requested as text, otherwise empty | Sample message contents |
| MessageBytes         | byte[]               | Message contents if requested as bytes, otherwise empty | Byte array with length of 14664,027 KB |||
| Message Properties   | MessageProperties    | Message properties | |
| Message Descriptor   | MessageDescriptor    | Message descriptor properties |||
| RFH2 Headers         | RFH2Headers          | RFH2 Headers if present | |

#### MessageProperties

See detailed information of all message properties from IBM MQ Documentation: <https://www.ibm.com/docs/en/ibm-mq/9.2?topic=interfaces-mqmessagenet-class>

| Property             | Type                 | Description                          | Example      |
| ---------------------| ---------------------| ------------------------------------ | -----        |
| TotalMessageLength   | string               | TotalMessageLength in bytes          | |
| PutDateTime          | DateTime             | The put timestamp of the message     | 2022-01-03T15:32:33.23+02:00  |
| PutApplicationType   | int                  | PutApplicationType                   | |
| PutApplicationName   | string               | PutApplicationName                   | |
| MessageSequenceNumber| int                  | MessageSequenceNumber                | |
| ApplicationOriginData| string               | ApplicationOriginData                | |
| ApplicationIdData    | string               | ApplicationIdData                    | |
| UserId               | string               | UserId                               | |
| PropertyValidation   | int                  | PropertyValidation                   | |
| OriginalLength       | int                  | OriginalLength                       | |
| MessageType          | int                  | MessageType                          | |
| MessageLength        | int                  | MessageLength                        | |
| MessageId            | string               | MessageId                            | |
| MessageFlags         | int                  | MessageFlags                         | |
| GroupId              | string               | GroupId                              | |
| DataOffset           | int                  | DataOffset                           | |
| DataLength           | int                  | DataLength                           | |
| CorrelationId        | string               | CorrelationId                        | |
| CharacterSet         | int                  | Character Set of the message (if requested as text). Common values: 1208 = UTF-8, 819 = ISO-8859-1, 1252 = Windows Latin1, 1200 = Unicode, see all values from IBM MQ Documentation: <https://www.ibm.com/docs/en/ibm-mq/9.2?topic=interfaces-character-set-identifiers-net-applications>                         | |
| BackoutCount         | int                  | BackoutCount                         | |
| AccountingToken      | string               | AccountingToken                      | |
| Version              | int                  | Version                              | |
| Report               | int                  | Report                               | |
| ReplyToQueueName     | string               | ReplyToQueueName                     | |
| ReplyToQueueManagerName | string            | ReplyToQueueManagerName              | |
| Priority             | int                  | Priority                             | |
| Persistence          | int                  | Persistence                          | |
| Offset               | int                  | Offset                               | |
| Format               | string               | Format                               | |
| Feedback             | int                  | Feedback                             | |
| Expiry               | int                  | Expiry                               | |
| Encoding             | int                  | Encoding                             | |

#### MessageDecriptor

See detailed information of all message descriptor properties from IBM MQ Documentation: <https://www.ibm.com/docs/en/ibm-mq/9.2?topic=descriptor-fields-mqmd>

| Property            | Type                 | Description                          | Example      |
| --------------------| ---------------------| ------------------------------------ | -----        |
| StrucId             | string               | Structure identifier                 | |
| Version             | int                  | Structure version number             | |
| Report              | int                  | Options for report messages          | |
| MsgType             | int                  | Message type                         | |
| Expiry              | int                  | Message lifetime                     | |
| Feedback            | int                  | Feedback or reason code              | |
| Encoding            | int                  | Numeric encoding of message data     | |
| CodedCharSetId      | int                  | Character set identifier of message data | |
| Format              | int                  | Format name of message data          | |
| Priority            | int                  | Message priority                     | |
| Persistence         | int                  | Message persistence                  | |
| MsgId               | byte[]               | Message identifier                   | |
| CorrelId            | byte[]               | Correlation identifier               | |
| BackoutCount        | int                  | Backout counter                      | |
| ReplyToQ            | string               | Name of reply queue                  | |
| ReplyToQMgr         | string               | Name of reply queue manager          | |
| UserIdentifier      | string               | User identifier                      | |
| AccountingToken     | byte[]               | Accounting token                     | |
| ApplIdentityData    | string               | Application data relating to identity | |
| PutApplType         | int                  | Type of application that put the message | |
| PutApplName         | string               | Name of application that put the message | |
| PutDate             | string               | Date when message was put                | |
| PutTime             | string               | Time when message was put                | |
| ApplOriginData      | string               | Application data relating to origin      | |
| GroupId             | byte[]               | Group identifier                         | |
| MsgSeqNumber        | int                  | Sequence number of logical message within group | |
| Offset              | int                  | Offset of data in physical message from start of logical message | |
| MsgFlags            | int                  | Message flags                        | |
| OriginalLength      | int                  | Length of original message           | |

#### RFH2Headers

See detailed information of all RFH2 headers from IBM MQ Documentation: <https://www.ibm.com/docs/en/ibm-mq/9.2?topic=2-initial-values-language-declarations-mqrfh2>

| Property            | Type                 | Description                          | Example      |
| --------------------| ---------------------| ------------------------------------ | -----        |
| StrucId             | string               | StrucId                              | RFH¬         |
| Version             | int                  | Version                              | 2            |
| StrucLength         | int                  | StrucLength including NameValue-fields | 36         |
| Encoding            | int                  | Encoding                             | 546          |
| CodedCharSetId      | int                  | CodedCharSetId                       | -2           |
| Format              | string               | Format                               | MQSTR        |
| Flags               | int                  | Flags                                | 0            |
| NameValueCCSID      | int                  | NameValueCCSID                       | 1208         |
| NameValueLength     | int                  | NameValueLength                      | |
| NameValueData       | string               | NameValueData                        | |

## PeekMessage

Get the first message from the queue without removing it from the queue.

### Input

| Property             | Type                 | Description                          | Example      |
| ---------------------| ---------------------| ------------------------------------ | -----        |
| Queue                | string               | Queue name to get messages from      | DEV.QUEUE.1  |
| MessageAsBytes       | bool                 | If set, return messsage as bytes, message in MessageBytes-field | No |
| Get Message Properties | bool               | If set, return properties in MessageProperties-field | Yes |
| Get Message Descriptor | bool               | If set, return properties in MessageDescriptor-field | No |
| Parse RFH2 Header    | bool                 | If set, parse and return RHF2 headers in RFH2Headers-field. Field is null if RFH2 headers was not found. If RFH2 Headers are parsed and returned, they are not visible in the actual Message returned. | No |

### Connection

| Property             | Type                 | Description                          | Example          |
| ---------------------| ---------------------| -------------------------------------| -----            |
| HostName             | string               | Host name of the IBM MQ instance     | localhost        |
| PortNumber           | int                  | Port number of the IBM MQ instance   | 1414             |
| Channel              | string               | Name of the channel                  | SAMPLE.CHANNEL   |
| QueueManagerName     | string               | Name of the Queue manager            | QM1              |
| UserID               | string               | UserID to be used for authentication & authorization (optional) | app |
| Password             | string               | Password of the UserID above (optional) | ****** |

### Options

| Property             | Type                 | Description                          | Example  |
| ---------------------| ---------------------| ------------------------------------ | -----    |
| SslCertStore         | string               | Cert store to use                    | MyStore  |
| SslCipherSpec        | string               | Cipher spec to use                   | TODO     |
| SslPeerName          | string               | Peer name to use                     | TODO     |
| SslResetCount        | int                  | Reset count                          | 0        |
| SslCertRevocationCheck | bool               | Cert revocation check performed      | No       |

### Returns

| Property             | Type                 | Description                          | Example  |
| ---------------------| ---------------------| ------------------------------------ | ---------|
| MessageFound         | bool                 | True/False, indicating whether a message was found | true |
| Data                 | QueueMessage         | A QueueMessage-object containing Message, MessageBytes, MessageProperties, MessageDescriptor and RFH2Headers (possibly as null) ||

#### QueueMessage

| Property             | Type                 | Description                          | Example  |
| ---------------------| ---------------------| ------------------------------------ | ---------|
| Message              | string               | Message contents if requested as text, otherwise empty | Sample message contents |
| MessageBytes         | byte[]               | Message contents if requested as bytes, otherwise empty | Byte array with length of 14664,027 KB |||
| Message Properties   | MessageProperties    | Message properties | |
| Message Descriptor   | MessageDescriptor    | Message descriptor properties |||
| RFH2 Headers         | RFH2Headers          | RFH2 Headers if present | |

#### MessageProperties

See detailed information of all message properties from IBM MQ Documentation: <https://www.ibm.com/docs/en/ibm-mq/9.2?topic=interfaces-mqmessagenet-class>

| Property             | Type                 | Description                          | Example      |
| ---------------------| ---------------------| ------------------------------------ | -----        |
| TotalMessageLength   | string               | TotalMessageLength in bytes          | |
| PutDateTime          | DateTime             | The put timestamp of the message     | 2022-01-03T15:32:33.23+02:00  |
| PutApplicationType   | int                  | PutApplicationType                   | |
| PutApplicationName   | string               | PutApplicationName                   | |
| MessageSequenceNumber| int                  | MessageSequenceNumber                | |
| ApplicationOriginData| string               | ApplicationOriginData                | |
| ApplicationIdData    | string               | ApplicationIdData                    | |
| UserId               | string               | UserId                               | |
| PropertyValidation   | int                  | PropertyValidation                   | |
| OriginalLength       | int                  | OriginalLength                       | |
| MessageType          | int                  | MessageType                          | |
| MessageLength        | int                  | MessageLength                        | |
| MessageId            | string               | MessageId                            | |
| MessageFlags         | int                  | MessageFlags                         | |
| GroupId              | string               | GroupId                              | |
| DataOffset           | int                  | DataOffset                           | |
| DataLength           | int                  | DataLength                           | |
| CorrelationId        | string               | CorrelationId                        | |
| CharacterSet         | int                  | Character Set of the message (if requested as text). Common values: 1208 = UTF-8, 819 = ISO-8859-1, 1252 = Windows Latin1, 1200 = Unicode, see all values from IBM MQ Documentation: <https://www.ibm.com/docs/en/ibm-mq/9.2?topic=interfaces-character-set-identifiers-net-applications>                         | |
| BackoutCount         | int                  | BackoutCount                         | |
| AccountingToken      | string               | AccountingToken                      | |
| Version              | int                  | Version                              | |
| Report               | int                  | Report                               | |
| ReplyToQueueName     | string               | ReplyToQueueName                     | |
| ReplyToQueueManagerName | string            | ReplyToQueueManagerName              | |
| Priority             | int                  | Priority                             | |
| Persistence          | int                  | Persistence                          | |
| Offset               | int                  | Offset                               | |
| Format               | string               | Format                               | |
| Feedback             | int                  | Feedback                             | |
| Expiry               | int                  | Expiry                               | |
| Encoding             | int                  | Encoding                             | |

#### MessageDecriptor

See detailed information of all message descriptor properties from IBM MQ Documentation: <https://www.ibm.com/docs/en/ibm-mq/9.2?topic=descriptor-fields-mqmd>

| Property            | Type                 | Description                          | Example      |
| --------------------| ---------------------| ------------------------------------ | -----        |
| StrucId             | string               | Structure identifier                 | |
| Version             | int                  | Structure version number             | |
| Report              | int                  | Options for report messages          | |
| MsgType             | int                  | Message type                         | |
| Expiry              | int                  | Message lifetime                     | |
| Feedback            | int                  | Feedback or reason code              | |
| Encoding            | int                  | Numeric encoding of message data     | |
| CodedCharSetId      | int                  | Character set identifier of message data | |
| Format              | int                  | Format name of message data          | |
| Priority            | int                  | Message priority                     | |
| Persistence         | int                  | Message persistence                  | |
| MsgId               | byte[]               | Message identifier                   | |
| CorrelId            | byte[]               | Correlation identifier               | |
| BackoutCount        | int                  | Backout counter                      | |
| ReplyToQ            | string               | Name of reply queue                  | |
| ReplyToQMgr         | string               | Name of reply queue manager          | |
| UserIdentifier      | string               | User identifier                      | |
| AccountingToken     | byte[]               | Accounting token                     | |
| ApplIdentityData    | string               | Application data relating to identity | |
| PutApplType         | int                  | Type of application that put the message | |
| PutApplName         | string               | Name of application that put the message | |
| PutDate             | string               | Date when message was put                | |
| PutTime             | string               | Time when message was put                | |
| ApplOriginData      | string               | Application data relating to origin      | |
| GroupId             | byte[]               | Group identifier                         | |
| MsgSeqNumber        | int                  | Sequence number of logical message within group | |
| Offset              | int                  | Offset of data in physical message from start of logical message | |
| MsgFlags            | int                  | Message flags                        | |
| OriginalLength      | int                  | Length of original message           | |

#### RFH2Headers

See detailed information of all RFH2 headers from IBM MQ Documentation: <https://www.ibm.com/docs/en/ibm-mq/9.2?topic=2-initial-values-language-declarations-mqrfh2>

| Property            | Type                 | Description                          | Example      |
| --------------------| ---------------------| ------------------------------------ | -----        |
| StrucId             | string               | StrucId                              | RFH¬         |
| Version             | int                  | Version                              | 2            |
| StrucLength         | int                  | StrucLength including NameValue-fields | 36         |
| Encoding            | int                  | Encoding                             | 546          |
| CodedCharSetId      | int                  | CodedCharSetId                       | -2           |
| Format              | string               | Format                               | MQSTR        |
| Flags               | int                  | Flags                                | 0            |
| NameValueCCSID      | int                  | NameValueCCSID                       | 1208         |
| NameValueLength     | int                  | NameValueLength                      | |
| NameValueData       | string               | NameValueData                        | |

## PutMessage

Put a message to a queue.

### Input

| Property             | Type                 | Description                          | Example      |
| ---------------------| ---------------------| ------------------------------------ | -----        |
| Queue                | string               | Queue name to get messages from      | DEV.QUEUE.1  |
| MessageAsBytes       | bool                 | If set, sent messsage as bytes       | No           |
| MessageContent       | string               | If MessageAsBytes set to No, this field used for message contents  | Sample message |
| MessageContentBytes  | byte[]               | If MessageAsBytes set to Yes, this field used for message contents | System.Text.Encoding.UTF8.GetBytes("Text to be converted")|

### Connection

| Property             | Type                 | Description                          | Example          |
| ---------------------| ---------------------| -------------------------------------| -----            |
| HostName             | string               | Host name of the IBM MQ instance     | localhost        |
| PortNumber           | int                  | Port number of the IBM MQ instance   | 1414             |
| Channel              | string               | Name of the channel                  | SAMPLE.CHANNEL   |
| QueueManagerName     | string               | Name of the Queue manager            | QM1              |
| UserID               | string               | UserID to be used for authentication & authorization (optional) | app |
| Password             | string               | Password of the UserID above (optional) | ****** |

### Options

| Property             | Type                 | Description                          | Example  |
| ---------------------| ---------------------| ------------------------------------ | -----    |
| SslCertStore         | string               | Cert store to use                    | MyStore  |
| SslCipherSpec        | string               | Cipher spec to use                   | |
| SslPeerName          | string               | Peer name to use                     | |
| SslResetCount        | int                  | Reset count                          | 0        |
| SslCertRevocationCheck | bool               | Cert revocation check performed      | No       |

### Message properties

| Property             | Type                 | Description                          | Example  |
| ---------------------| ---------------------| ------------------------------------ | -----    |
| Chracter set         | enum                 | Chracter set (encoding) to be used if message sent as text, values: UTF8, ISO88591, WindowsLatin1, Unicode, Other, Automatic. If set to Other you can give any valid integer character set value | UTF8  |
| Chracter set value   | int                  | If previous parameter set to Other, you can give any valid character set value here | 819 |
| Properties          | array of Name-Value -pairs | Message properties as Name-Value pairs, Value can be different types (int, string, byte[] depending on property). See possible values and types from IBM MQ Documentation: https://www.ibm.com/docs/en/ibm-mq/9.2?topic=interfaces-mqmessagenet-class. **NB**: The following properties cannot be set: *TotalMessageLength, MessageLength, DataLength, BackoutCount* | Report : 1, ReplyToQueueName : REPORT.QUEUE     |
| Descriptors         | array of Name-Value -pairs |  Message descriptors as Name-Value pairs, Value can be different types (int, string, byte[] depending on property). See possible values and types from IBM MQ Documentation: https://www.ibm.com/docs/en/ibm-mq/9.2?topic=descriptor-fields-mqmd **NB**: The following properties cannot be set: *StructMQMD* | Report : 2 |
| Set RFH2 Headers | bool               | Whether to set RFH2 headers to the message or not | No       |
| RFH2 Headers | array of Name-Value -pairs | If previous parameter set to Yes, RFH2 headers will be written to the message using default values or explicit values given here. Only one NameValueData-field is supported, NameValueLength should not be given, it will be calculated based on the contents of NameValueData. See header names, types and default values from IBM MQ Documentation: https://www.ibm.com/docs/en/ibm-mq/9.2?topic=2-initial-values-language-declarations-mqrfh2 | Format : MQSTR, NameValueData : ```<mcd><Msd>text</Msd></mcd>```|


### Returns

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | -----   |
| Success              | bool                 | True/False indicating the result of the Put operation, true if it was successful | true |

# License

This project is licensed under the MIT License - see the LICENSE file for details

# Building

Clone a copy of the repo

`git clone https://github.com/CommunityHiQ/Frends.Community.IBMMQ.git`

Restore dependencies

`nuget restore frends.community.ibmmq`

Rebuild the project

Run Tests with nunit3. Tests can be found under

`Frends.Community.IBMMQ.Tests\bin\Release\Frends.Community.IBMMQ.Tests.dll`

Create a nuget package

`nuget pack nuspec/Frends.Community.IBMMQ.nuspec -properties Configuration=Release`

# Contributing
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

# Change Log

| Version              | Changes                 |
| ---------------------| ------------------------|
| 1.1.0                | Initial version of IBMMQ Tasks |