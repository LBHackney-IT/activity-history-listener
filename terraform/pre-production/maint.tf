terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 3.0"
    }
  }
}

provider "aws" {
  region = "eu-west-2"
}

terraform {
  backend "s3" {
    bucket         = "housing-pre-production-terraform-state"
    encrypt        = true
    region         = "eu-west-2"
    key            = "services/activity-listener/state"
    dynamodb_table = "housing-pre-production-terraform-state-lock"
  }
}

data "aws_vpc" "housing_pre_production_vpc" {
  tags = {
    Name = "housing-pre-prod-pre-prod"
  }
}

module "activity_listener_sg" {
  source             = "../modules/security_groups/outbound_only_traffic"
  vpc_id             = data.aws_vpc.housing_pre_production_vpc.id
  user_resource_name = "activity_listener"
  environment_name   = var.environment_name
}


# Topic parameters were removed with the upstream pre-production services.
# The queue policy and topic subscriptions remain in state and are destroyed
# with the rest of this configuration. Destroy does not read those parameters.

resource "aws_sqs_queue" "activity_history_dead_letter_queue" {
  name                              = "activityhistorydeadletterqueue.fifo"
  fifo_queue                        = true
  content_based_deduplication       = true
  kms_master_key_id                 = "alias/housing-pre-production-cmk"
  kms_data_key_reuse_period_seconds = 300
}

resource "aws_sqs_queue" "activity_history_queue" {
  name                              = "activityhistoryqueue.fifo"
  fifo_queue                        = true
  content_based_deduplication       = true
  kms_master_key_id                 = "alias/housing-pre-production-cmk"
  kms_data_key_reuse_period_seconds = 300
  redrive_policy = jsonencode({
    deadLetterTargetArn = aws_sqs_queue.activity_history_dead_letter_queue.arn,
    maxReceiveCount     = 3
  })
}

resource "aws_ssm_parameter" "activity_history_sqs_queue_arn" {
  name  = "/sqs-queue/pre-production/activity-history/arn"
  type  = "String"
  value = aws_sqs_queue.activity_history_queue.arn
}
