# Sir.Store

In-process document database.

## File format

### .val

	val val val... 

### .key

	key key key... 

### .doc

	key_id val_id...

### .vix

	(val_id) val_offset val_len val_type

### .kix

	(key_id) key_offset key_len key_type

### .dix

	(doc_id) doc_offset doc_len

### .pos

	[doc_id doc_id next_page_offset] [doc_id       ]...

### .kmap

	(key_id) key_hash

### .ix
### .vec