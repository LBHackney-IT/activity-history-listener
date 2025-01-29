# Activity History Listener
Listener application implementing an AWS function to receive messages that will result in an entity activity log.

This application subscribes to all of the SNS topics for housing solutions related events.
For audit log purposes, it records to the ActivityHistory DynamoDB table the occurence of each event that was published tosome SNS topic for other listener applications to consume.
Some of this recorded activity _(such as update operations)_ can also be looked up via micro-frontend page for each entity managed by Manage My Home solution.
