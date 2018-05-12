# Sir.Store

Sir.Store is a document storage engine. Documents have strings for keys and ´IComparable´ for values.

Read, update and delete operations can be issued on single or multiple records by specifying a key/value filter.

## File format

### collection_hash.pos

	doc_id doc_id doc_id...

### collection_hash.val

	val val val... 

### collection_hash.key

	key key key... 

### collection_hash.vix

	(val_id) val_type val_len pos_offset pos_len

### collection_hash.map

	key_id val_id 

### collection_hash.dix

	(doc_id) map_offset map_len 

### collection_hash.pix

	(key_id) key_offset key_len