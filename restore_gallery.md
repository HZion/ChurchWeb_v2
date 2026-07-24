# Gallery Data Restoration Plan

## Current Situation
- Backup file: `C:\Users\28400\Desktop\ChurchWeb\chuch_sc_jv0j` (PostgreSQL custom format dump)
- Expected: ~38 albums in backup
- Current DB: Only 2 sample albums in `churchweb.Albums` and `churchweb.AlbumPhotos`

## Restoration Strategy

### Option 1: Use pg_restore (Recommended if available)
```bash
# Need to install PostgreSQL client tools first
pg_restore -h <host> -U <user> -d <database> \
  --schema=public \
  -t Albums -t AlbumPhotos \
  "C:\Users\28400\Desktop\ChurchWeb\chuch_sc_jv0j"
```

### Option 2: Convert to SQL and restore via C#
Since pg_restore is not available, we'll create a C# utility to:
1. Extract SQL from the backup
2. Read gallery table data
3. Insert into churchweb schema with proper mapping

## Safety Rules (MUST FOLLOW)
1. ✅ READ-ONLY on public schema
2. ✅ WRITE only to churchweb schema
3. ✅ Use environment variable for connection string
4. ✅ Idempotent: Track source records to prevent duplicates
5. ✅ One-time manual execution (not auto-run on startup)

## Next Steps
1. Check if pg_restore can be installed
2. If not, create C# restoration utility
3. Test on a small dataset first
4. Execute full restoration
