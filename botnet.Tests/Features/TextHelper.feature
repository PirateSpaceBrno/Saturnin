Feature: TextHelper

Scenario Outline: Test Convert to ASCII method
	When I convert '<withDiacritics>' to ASCII
	Then Converted text is '<ascii>'
        Examples: 
        | withDiacritics                               | ascii                                        |
        | Příliš žluťoučký kůň úpěl ďábelské ódy.      | Prilis zlutoucky kun upel dabelske ody.      |
        | áäčďéěëíµňôóöŕřšťúůüýžÁÄČĎÉĚËÍĄŇÓÖÔŘŔŠŤÚŮÜÝŽ | aacdeeeiµnooorrstuuuyzAACDEEEIANOOORRSTUUUYZ |
