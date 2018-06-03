Feature: GraphApi

Scenario Outline: Test Saturnin responses
	When I receive fake message with text '<receivedMessage>' from '<receivedFrom>' in group '<receivedGroup>'
	Then Saturnin respond with message '<sendMessage>' to '<sendTo>' in group '<sendGroup>'
	Examples: 
		| receivedMessage                                                                | receivedFrom  | receivedGroup | sendMessage                                                                              | sendTo        | sendGroup |
		| Saturnine, kolik členů má KS Jihomoravský kraj?                                | +420608828650 |               | Skupina 'KS Jihomoravský kraj' má k tomuto okamžiku 51 členů.                            | +420608828650 |           |
		| Saturnine, kolik členů má KS Jihomoravský kraj?                                | +420608828650 |               | Skupina 'KS Jihomoravský kraj' má k tomuto okamžiku 50 členů.                            | +420608828650 |           |
		| Saturnine, řekni vtip                                                          | +420608828650 |               |                                                                                          |               |           |
		| Saturnine, v 1:00 pošli zprávu s textem 'Ahoj zmetku' na číslo '+420608828650' | +420608828650 |               | Odeslání zprávy na číslo '+420608828650' mám naplánováno a provedu v 6/4/2018 1:00:00 AM | +420608828650 |           |
