SELECT username, password FROM admins

UPDATE admins 
SET password = 'dc4ce0d904f2147ada12ef11c4fff11f643afffbcf3e78546cf1483a405fc3ef'
WHERE username = 'admin'

SELECT * FROM admins