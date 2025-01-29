# Activity History Listener
Listener application implementing an AWS function to receive messages that will result in an entity activity log.

## Implementation
This application subscribes to all of the SNS topics for housing solutions related events.

It records to the 'ActivityHistory' DynamoDB table the occurence of each event that was published to an SNS topic for other listener applications to consume.

## What for?
These events are recorded to 'ActivityHistory' table:
* For audit log purposes - some of this recorded activity _(such as update operations)_ can also be looked up via micro-frontend page for each entity managed by Manage My Home solution. When any uncertainty arises for why certain entity's data is not shown as expected, audit logs allows seeing the name of the officer who last changed that data, as well as what that data was before the change was made.
* We've had some cases where events either fail to get processed, or flat out seemingly disappear. Activity history table allows us to find these failed events & helps us debug the problem, as well as helps us fix data to what it should be.
