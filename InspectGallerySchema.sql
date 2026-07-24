-- Step 1: Find all tables in public schema that might be gallery-related
SELECT
    table_name,
    table_schema
FROM information_schema.tables
WHERE table_schema = 'public'
  AND (
    table_name ILIKE '%gallery%'
    OR table_name ILIKE '%photo%'
    OR table_name ILIKE '%album%'
    OR table_name ILIKE '%image%'
  )
ORDER BY table_name;

-- Step 2: Get all table names in public schema (if no gallery-specific tables found)
SELECT
    table_name,
    table_schema
FROM information_schema.tables
WHERE table_schema = 'public'
ORDER BY table_name;
