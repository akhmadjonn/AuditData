-- ============================================
-- AUDIT DATA REPORT QUERIES
-- PostgreSQL Scripts for Manual Execution
-- ============================================

-- ============================================
-- 1. ALREADY PAID ORDERS
-- ============================================

-- Case 1: Matched in Pmt sheet by Load Number
SELECT
    s.*,
    p.*,
    'Pmt Match' as match_type,
    (s."InvoicedAmount" - p."InvoiceAmount") as difference
FROM "Spreadsheets" s
INNER JOIN "Payments" p ON s."LoadId" = p."LoadNumber"
WHERE p."SheetName" = 'Pmt';

-- Case 2: Matched in NF sheet by Invoice Number
SELECT
    s.*,
    p.*,
    'NF Match' as match_type,
    (s."InvoicedAmount" - p."InvoiceAmount") as difference
FROM "Spreadsheets" s
INNER JOIN "Payments" p ON s."LoadId" = p."InvoiceNumber"
WHERE p."SheetName" = 'NF';

-- Case 3: Matched in OP SP sheet with full payment
SELECT
    s.*,
    p.*,
    'OP SP Full Payment' as match_type,
    (s."InvoicedAmount" - p."InvoiceAmount") as difference
FROM "Spreadsheets" s
INNER JOIN "Payments" p ON s."LoadId" = p."LoadNumber"
WHERE p."SheetName" = 'OP  SP'
  AND p."InvoiceAmount" >= s."InvoicedAmount";

-- Combined Already Paid Orders (All 3 cases)
SELECT * FROM (
    -- Case 1
    SELECT
        s.*,
        p."Id" as payment_id,
        p."InvoiceNumber",
        p."LoadNumber",
        p."PurchaseDate",
        p."PaymentDate",
        p."CheckNumber",
        p."DebtorName",
        p."FeeDays",
        p."InvoiceAmount",
        p."ActivityType",
        p."CheckAmount",
        p."SheetName",
        'Pmt Match' as match_type,
        (s."InvoicedAmount" - p."InvoiceAmount") as difference
    FROM "Spreadsheets" s
    INNER JOIN "Payments" p ON s."LoadId" = p."LoadNumber"
    WHERE p."SheetName" = 'Pmt'

    UNION

    -- Case 2
    SELECT
        s.*,
        p."Id" as payment_id,
        p."InvoiceNumber",
        p."LoadNumber",
        p."PurchaseDate",
        p."PaymentDate",
        p."CheckNumber",
        p."DebtorName",
        p."FeeDays",
        p."InvoiceAmount",
        p."ActivityType",
        p."CheckAmount",
        p."SheetName",
        'NF Match' as match_type,
        (s."InvoicedAmount" - p."InvoiceAmount") as difference
    FROM "Spreadsheets" s
    INNER JOIN "Payments" p ON s."LoadId" = p."InvoiceNumber"
    WHERE p."SheetName" = 'NF'

    UNION

    -- Case 3
    SELECT
        s.*,
        p."Id" as payment_id,
        p."InvoiceNumber",
        p."LoadNumber",
        p."PurchaseDate",
        p."PaymentDate",
        p."CheckNumber",
        p."DebtorName",
        p."FeeDays",
        p."InvoiceAmount",
        p."ActivityType",
        p."CheckAmount",
        p."SheetName",
        'OP SP Full Payment' as match_type,
        (s."InvoicedAmount" - p."InvoiceAmount") as difference
    FROM "Spreadsheets" s
    INNER JOIN "Payments" p ON s."LoadId" = p."LoadNumber"
    WHERE p."SheetName" = 'OP  SP'
      AND p."InvoiceAmount" >= s."InvoicedAmount"
) already_paid
ORDER BY "LoadId";


-- ============================================
-- 2. SHORT PAID ORDERS
-- ============================================

SELECT
    s.*,
    p."Id" as payment_id,
    p."InvoiceNumber",
    p."LoadNumber",
    p."PurchaseDate",
    p."PaymentDate",
    p."CheckNumber",
    p."DebtorName",
    p."FeeDays",
    p."InvoiceAmount",
    p."ActivityType",
    p."CheckAmount",
    p."SheetName",
    'Short Payment' as match_type,
    (s."InvoicedAmount" - p."InvoiceAmount") as difference
FROM "Spreadsheets" s
INNER JOIN "Payments" p ON s."LoadId" = p."LoadNumber"
WHERE p."SheetName" = 'OP  SP'
  AND p."InvoiceAmount" < s."InvoicedAmount"
ORDER BY difference DESC;


-- ============================================
-- 3. CHARGE BACK ORDERS
-- ============================================

SELECT
    s.*,
    p."Id" as payment_id,
    p."InvoiceNumber",
    p."LoadNumber",
    p."PurchaseDate",
    p."PaymentDate",
    p."CheckNumber",
    p."DebtorName",
    p."FeeDays",
    p."InvoiceAmount",
    p."ActivityType",
    p."CheckAmount",
    p."SheetName",
    'Charge Back' as match_type,
    (s."InvoicedAmount" - p."InvoiceAmount") as difference
FROM "Spreadsheets" s
INNER JOIN "Payments" p ON s."LoadId" = p."LoadNumber"
WHERE p."SheetName" = 'CB'
ORDER BY s."LoadId";


-- ============================================
-- 4. UNPAID ORDERS
-- ============================================

SELECT
    s.*,
    'Unpaid' as match_type
FROM "Spreadsheets" s
WHERE NOT EXISTS (
    -- Not in Pmt
    SELECT 1 FROM "Payments" p
    WHERE p."LoadNumber" = s."LoadId"
    AND p."SheetName" = 'Pmt'
)
AND NOT EXISTS (
    -- Not in NF
    SELECT 1 FROM "Payments" p
    WHERE p."InvoiceNumber" = s."LoadId"
    AND p."SheetName" = 'NF'
)
AND NOT EXISTS (
    -- Not in OP SP (full payment)
    SELECT 1 FROM "Payments" p
    WHERE p."LoadNumber" = s."LoadId"
    AND p."SheetName" = 'OP  SP'
    AND p."InvoiceAmount" >= s."InvoicedAmount"
)
AND NOT EXISTS (
    -- Not in OP SP (short payment)
    SELECT 1 FROM "Payments" p
    WHERE p."LoadNumber" = s."LoadId"
    AND p."SheetName" = 'OP  SP'
    AND p."InvoiceAmount" < s."InvoicedAmount"
)
AND NOT EXISTS (
    -- Not in CB
    SELECT 1 FROM "Payments" p
    WHERE p."LoadNumber" = s."LoadId"
    AND p."SheetName" = 'CB'
)
ORDER BY s."LoadId";


-- ============================================
-- SUMMARY STATISTICS
-- ============================================

-- Count of orders in each category
SELECT
    'Already Paid Orders' as category,
    COUNT(*) as count
FROM (
    SELECT s."Id"
    FROM "Spreadsheets" s
    INNER JOIN "Payments" p ON s."LoadId" = p."LoadNumber"
    WHERE p."SheetName" = 'Pmt'

    UNION

    SELECT s."Id"
    FROM "Spreadsheets" s
    INNER JOIN "Payments" p ON s."LoadId" = p."InvoiceNumber"
    WHERE p."SheetName" = 'NF'

    UNION

    SELECT s."Id"
    FROM "Spreadsheets" s
    INNER JOIN "Payments" p ON s."LoadId" = p."LoadNumber"
    WHERE p."SheetName" = 'OP  SP'
      AND p."InvoiceAmount" >= s."InvoicedAmount"
) t

UNION ALL

SELECT
    'Short Paid Orders' as category,
    COUNT(*) as count
FROM "Spreadsheets" s
INNER JOIN "Payments" p ON s."LoadId" = p."LoadNumber"
WHERE p."SheetName" = 'OP  SP'
  AND p."InvoiceAmount" < s."InvoicedAmount"

UNION ALL

SELECT
    'Charge Back Orders' as category,
    COUNT(*) as count
FROM "Spreadsheets" s
INNER JOIN "Payments" p ON s."LoadId" = p."LoadNumber"
WHERE p."SheetName" = 'CB'

UNION ALL

SELECT
    'Unpaid Orders' as category,
    COUNT(*) as count
FROM "Spreadsheets" s
WHERE NOT EXISTS (
    SELECT 1 FROM "Payments" p
    WHERE (p."LoadNumber" = s."LoadId" AND p."SheetName" = 'Pmt')
       OR (p."InvoiceNumber" = s."LoadId" AND p."SheetName" = 'NF')
       OR (p."LoadNumber" = s."LoadId" AND p."SheetName" = 'OP  SP')
       OR (p."LoadNumber" = s."LoadId" AND p."SheetName" = 'CB')
);


-- Total Amounts Summary
SELECT
    'Already Paid Orders' as category,
    SUM(s."InvoicedAmount") as total_invoiced,
    SUM(p."InvoiceAmount") as total_paid,
    SUM(s."InvoicedAmount" - p."InvoiceAmount") as total_difference
FROM (
    SELECT DISTINCT ON (s."Id") s.*, p."InvoiceAmount"
    FROM "Spreadsheets" s
    INNER JOIN "Payments" p ON (
        (s."LoadId" = p."LoadNumber" AND p."SheetName" = 'Pmt')
        OR (s."LoadId" = p."InvoiceNumber" AND p."SheetName" = 'NF')
        OR (s."LoadId" = p."LoadNumber" AND p."SheetName" = 'OP  SP' AND p."InvoiceAmount" >= s."InvoicedAmount")
    )
) t
INNER JOIN "Spreadsheets" s ON s."Id" = t."Id"
INNER JOIN "Payments" p ON (
    (s."LoadId" = p."LoadNumber" AND p."SheetName" = 'Pmt')
    OR (s."LoadId" = p."InvoiceNumber" AND p."SheetName" = 'NF')
    OR (s."LoadId" = p."LoadNumber" AND p."SheetName" = 'OP  SP' AND p."InvoiceAmount" >= s."InvoicedAmount")
)

UNION ALL

SELECT
    'Short Paid Orders' as category,
    SUM(s."InvoicedAmount") as total_invoiced,
    SUM(p."InvoiceAmount") as total_paid,
    SUM(s."InvoicedAmount" - p."InvoiceAmount") as total_difference
FROM "Spreadsheets" s
INNER JOIN "Payments" p ON s."LoadId" = p."LoadNumber"
WHERE p."SheetName" = 'OP  SP'
  AND p."InvoiceAmount" < s."InvoicedAmount"

UNION ALL

SELECT
    'Charge Back Orders' as category,
    SUM(s."InvoicedAmount") as total_invoiced,
    SUM(p."InvoiceAmount") as total_paid,
    SUM(s."InvoicedAmount" - p."InvoiceAmount") as total_difference
FROM "Spreadsheets" s
INNER JOIN "Payments" p ON s."LoadId" = p."LoadNumber"
WHERE p."SheetName" = 'CB';
