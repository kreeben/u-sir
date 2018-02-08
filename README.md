# U-sir

U-sir is a map/reducer with read, write and data servers that you may run on a single machine or distribute over a cluster.

Features:

- Programmatically construct stored _map/reduce procedures_ and execute them from any HTTP client. 
- Configure the write pipeline by creating your own _analyzers_.
- Add support for your own data formats by authoring _model binders_. 
- Interface through HTTP API, divided into read and write endpoints, for executing the extendable read and write pipelines. 
- Support for any type of data format can be added in the form of custom model binders. 

## Full-text search

Built into U-sir is a full-text search engine. It consists of a map/reduce procedure, a query parser and an analyzer.

- Query parser lexes and parses queries.
- Map/reduce procedure scans index, fetches and aggregates search result.
- Write pipeline executes a full-text analyzer.

### Full-text analyzer

- Maintains a full-text search index of all the data
- Lexes, parses and semantically classifies all words.
- Links classes to table and column names to form a query language vocabulary.

## Content types

Model binders take care of the work needed to add support for a file format. A built-in model binder adds support for ASCII delimited text files, i.e. text files that separates records (rows) with ASCII char 30 ("record separator") and row members with ASCII chars 31 ("unit separator"). First row should include field labels.

### Other content types

To add support for other content types you may implement your own model binder. It should expose the data in the HTTP request stream to the analyzer as a list of tuples plus a tuple for the column labels. 

## Writing

Insert and update data by issuing HTTP POST commands to a writer endpoint with a data file as the payload.

Example 1: write command

	HTTP GET http://uwrite.company.com text/plain sales.txt

`http://uwrite.company.com` (url)
`text/plain` (media type)
`sales.txt` (payload)

The writer will

- append the payload (request stream) to a segment in a log file
- index the location in the log of each row of each file and register their state
- find a model binder mapped to media type 'text/plain' and run it over the request stream to get a model
- run all registered analyzers (IAnalyzer) over the model

## Reading

Read data by issuing HTTP GET commands to the read endpoint.

Example 2: read command using built-in search procedure

	HTTP GET http://uread.company.com/search?q=flights%20from%20copenhagen%20to%20paris

`http://` (protocol)
`uwrite.company.com` (host)
`search` (name of procedure or table)
`?q=flights%20from%20copenhagen%20to%20paris` (query)
