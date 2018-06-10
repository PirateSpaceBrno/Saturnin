Feature: Subscriber store

Scenario Outline: I am able to remove scheduled message
	When I add message scheduled on '<date>' from '<from>' to '<to>' with text '<message>'
	Then I am able to delete message scheduled  on '<date>' from '<from>' to '<to>' with text '<message>'
	Examples:
		| date  | from          | to            | message          |
		| 12:00 | +420608828650 | +420608123456 | testovací zpráva |

Scenario: I am able to remove DPMB line subscription
	Then I am able to delete DPMB line subscription