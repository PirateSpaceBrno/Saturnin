Feature: GeoLocationHelper

Scenario Outline: Test if point is inside circle
	Then Point '<lat1>''<lon1>' is '<insideOutside>' circle with center '<lat2>''<lon2>' and radius '<radius>'
        Examples: 
        | lat1       | lon1       | insideOutside | lat2       | lon2       | radius |
        | 49.1989733 | 16.5971546 | inside        | 49.1989733 | 16.5971546 | 10     |
		| 49.1989733 | 16.5971546 | inside        | 49.1989733 | 16.5971546 | 1      |
		| 49.1922918 | 16.6025405 | outside       | 49.1989733 | 16.5971546 | 1      |
		| 49.1908375 | 16.6049822 | outside       | 49.1989733 | 16.5971546 | 10     |
		| 49.1908375 | 16.6049822 | outside       | 49.1989733 | 16.5971546 | 100    |
		| 49.1908375 | 16.6049822 | inside        | 49.1989733 | 16.5971546 | 1000   |

