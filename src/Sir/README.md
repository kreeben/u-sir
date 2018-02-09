# U-sir

U-sir is a distributable map/reducer and search engine with a HTTP API and programmable read/write pipelines. 

Read and write data in any format. Model binders map between media types and file formats. Add support for any input/output by creating custom model binders and mapping them to a media type.

### Features

- Programmatically construct stored __map/reduce procedures__. 
- Create custom __model binders__ to supprt any file format.
- Configure the write pipeline by creating your own __analyzers__.

## Full-text search

Built into U-sir is a full-text search engine. It consists of a map/reduce procedure, a query parser and an analyzer.

- Query parser lexes and parses queries.
- Map/reduce procedure scans index, fetches and aggregates search result.
- Write pipeline executes a full-text analyzer.

### Full-text analyzer

- Maintains a full-text search index of all the data
- Lexes, parses and semantically classifies all words.
- Links classes to table and column names to form a query language vocabulary.

## Media types

Model binders take care of the work needed to add support for a file format. A built-in model binder adds support for ASCII delimited text files, i.e. text files that separates records (rows) with ASCII char 30 ("record separator") and row members with ASCII chars 31 ("unit separator"). First row should include field labels.

### Other content types

To add support for other content types you may implement your own model binder. It should expose the data in the HTTP request stream to the analyzer as a list of tuples plus a tuple for the column labels. 

## Writing

Insert and update data by issuing HTTP POST commands to a writer endpoint with a data file as the payload.

### Example 1: write command

	HTTP POST http://u-sir.company.com (text/plain, sales.txt)

#### Request explained

##### Protocol
`http://`

##### Host
`u-sir.company.com`

##### Content-type
`text/plain`

##### Payload
`sales.txt`

### The writer will

- append the payload (request stream) to a segment in a log file
- index the location in the log of each record register their state
- find a model binder mapped to media type 'text/plain' and run it over the request stream to get a model
- run all registered analyzers (IAnalyzer) over the model

## Reading

Read data by issuing HTTP GET commands to the read endpoint.

### Example 2: read command using built-in search procedure

	HTTP GET http://u-sir.company.com/search?q=flights%20from%20copenhagen%20to%20paris (text/plain)

#### Request explained

##### Protocol
`http://`

##### Host
`u-sir.company.com`

##### Name of procedure or table
`search`

##### Query
`?q=flights%20from%20copenhagen%20to%20paris`

##### Accept
`text/plain`

### Search map/reduce procedure will

- scan index
- fetch and aggregate result
- respond with output format corresponding to the "Accept" request header.
