#!/bin/bash

mediaPath=$1

if [ -z "$mediaPath" ] ; then
	echo "No media path provided."
	exit 1
fi

PSQL="psql mvcstore -tAc"
MYSQL="mysql mycms -Nsr -uroot -e"

versionDateTime=$($MYSQL "SELECT NOW();")

ids=$($PSQL 'SELECT id FROM "Product";')

for i in $ids ; do
	#get the product name and only take characters in ranges a-z or A-Z.
	prodName=$($PSQL "SELECT name FROM \"Product\" WHERE id=$i;" | perl -pe "s/\'/''/g" | perl -pe "s/\"/\"\"/g")
	catName=$($PSQL "SELECT \"Category\".name FROM \"Product\", \"Category\" WHERE \"Product\".id=$i AND \"Category\".id=\"Product\".category_id;" | perl -pe "s/\'/''/g" | perl -pe "s/\"/\"\"/g")
	prodDesc=$($PSQL "SELECT description FROM \"Product\" WHERE id=$i;" | perl -pe "s/\'/''/g" | perl -pe "s/\"/\"\"/g")
	prodPrice=$($PSQL "SELECT price FROM \"Product\" WHERE id=$i;")
	prodPrice=${prodPrice%.*}

	imgName=$(echo $prodName | perl -pe "s/[^A-Za-z]+//g")
	imgFileName="${imgName}.png"
	imgTempPath=$(mktemp)
	$PSQL "SELECT encode (Image, 'hex') FROM \"Product\" WHERE id=$i" | xxd -r -p - $imgTempPath
	
	###########
	
	###########
	# IMPORT of media starts now
	###########
	
	# add media (image) to UMBRACONODE
	sortOrder=$($MYSQL "SELECT MAX(SORTORDER) FROM UMBRACONODE WHERE NODEOBJECTTYPE='b796f64c-1f99-4ffb-b886-4bf4bc011a9c';")
	sortOrder=$(($sortOrder+1))
	
	guid=$(r=( $(openssl rand 100000 | sha1sum) ); printf "%s${r[0]:0:8}-${r[0]:8:4}-${r[0]:12:4}-${r[0]:16:4}-${r[0]:20:12}\n")
	
	imgId=$($MYSQL "INSERT INTO UMBRACONODE (PARENTID, NODEUSER, LEVEL, PATH, SORTORDER, UNIQUEID, TEXT, NODEOBJECTTYPE) VALUES (-1, 0, 1, '', $sortOrder, '$guid', '$imgName', 'b796f64c-1f99-4ffb-b886-4bf4bc011a9c'); SELECT LAST_INSERT_ID ();")
	$MYSQL "UPDATE UMBRACONODE SET PATH='-1,$imgId' WHERE ID=$imgId;"
	# media added to UMBRACONODE
	
	###########
	
	# setup version in CMSCONTENTVERSION
	imgVersionGuid=$(r=( $(openssl rand 100000 | sha1sum) ); printf "%s${r[0]:0:8}-${r[0]:8:4}-${r[0]:12:4}-${r[0]:16:4}-${r[0]:20:12}\n")
	$MYSQL "INSERT INTO CMSCONTENTVERSION (CONTENTID, VERSIONID, VERSIONDATE) VALUES ($imgId, '$imgVersionGuid', '$versionDateTime');"
	# finished setting up version in CMSCONTENTVERSION
	
	###########
	
	# add media to CMSCONTENT (1032 is the content type id for images)
	$MYSQL "INSERT INTO CMSCONTENT (NODEID, CONTENTTYPE) VALUES ($imgId, 1032);"
	# media added to CMSCONTENT
	
	###########
	
	# add image properties to CMSPROPERTYDATA
	pathId=$($MYSQL "INSERT INTO CMSPROPERTYDATA (CONTENTNODEID, VERSIONID, PROPERTYTYPEID) VALUES ($imgId, '$imgVersionGuid', 6); SELECT LAST_INSERT_ID ();")
	$MYSQL "UPDATE CMSPROPERTYDATA SET DATANVARCHAR='/media/${pathId}/${imgFileName}' WHERE ID=$pathId;"
	
	$MYSQL "INSERT INTO CMSPROPERTYDATA (CONTENTNODEID, VERSIONID, PROPERTYTYPEID, DATANVARCHAR) VALUES ($imgId, '$imgVersionGuid', 7, '250');"
	$MYSQL "INSERT INTO CMSPROPERTYDATA (CONTENTNODEID, VERSIONID, PROPERTYTYPEID, DATANVARCHAR) VALUES ($imgId, '$imgVersionGuid', 8, '250');"
	
	imgSize=$(stat -c%s $imgTempPath)
	$MYSQL "INSERT INTO CMSPROPERTYDATA (CONTENTNODEID, VERSIONID, PROPERTYTYPEID, DATANVARCHAR) VALUES ($imgId, '$imgVersionGuid', 9, '${imgSize}');"
	
	$MYSQL "INSERT INTO CMSPROPERTYDATA (CONTENTNODEID, VERSIONID, PROPERTYTYPEID, DATANVARCHAR) VALUES ($imgId, '$imgVersionGuid', 10, 'png');"
	# added image properties to CMSPROPERTYDATA
	
	###########
	
	# copy img file to proper location
	mkdir ${mediaPath}/${pathId}/
	cp $imgTempPath ${mediaPath}/${pathId}/${imgFileName}
	# copied img file to proper location
	
	###########
	
	###########
	# IMPORTING media finished
	# IMPORT of products starts now
	###########
	
	###########
	
	# check if category exists, if not, add it
	catId=$($MYSQL "SELECT id FROM UMBRACONODE WHERE TEXT='${catName}'")
	if [ -z $catId ] ; then
		# cat doesn't exist, create it
		sortOrder=$($MYSQL "SELECT MAX(SORTORDER) FROM UMBRACONODE WHERE NODEOBJECTTYPE='c66ba18e-eaf3-4cff-8a22-41b16d66a972' AND PARENTID=1050;")
		sortOrder=$(($sortOrder+1))

		guid=$(r=( $(openssl rand 100000 | sha1sum) ); printf "%s${r[0]:0:8}-${r[0]:8:4}-${r[0]:12:4}-${r[0]:16:4}-${r[0]:20:12}\n")

		catId=$($MYSQL "INSERT INTO UMBRACONODE (PARENTID, NODEUSER, LEVEL, PATH, SORTORDER, UNIQUEID, TEXT, NODEOBJECTTYPE) VALUES (1050, 0, 2, '', $sortOrder, '$guid', '$catName', 'c66ba18e-eaf3-4cff-8a22-41b16d66a972'); SELECT LAST_INSERT_ID ();")
		$MYSQL "UPDATE UMBRACONODE SET PATH='-1,1050,$catId' WHERE ID=$catId;"

		###########

		# setup version in CMSCONTENTVERSION
		catVersionGuid=$(r=( $(openssl rand 100000 | sha1sum) ); printf "%s${r[0]:0:8}-${r[0]:8:4}-${r[0]:12:4}-${r[0]:16:4}-${r[0]:20:12}\n")
		$MYSQL "INSERT INTO CMSCONTENTVERSION (CONTENTID, VERSIONID, VERSIONDATE) VALUES ($catId, '$catVersionGuid', '$versionDateTime');"
		# finished setting up version in CMSCONTENTVERSION

		###########

		# add cat to CMSCONTENT (1056 is the content type id for category pages)
		$MYSQL "INSERT INTO CMSCONTENT (NODEID, CONTENTTYPE) VALUES ($catId, 1056);"
		# cat added to CMSCONTENT

		###########
		
		# add cat properties to CMSPROPERTYDATA
		$MYSQL "INSERT INTO CMSPROPERTYDATA (CONTENTNODEID, VERSIONID, PROPERTYTYPEID, DATANVARCHAR) VALUES ($catId, '$catVersionGuid', 33, '${catName}');"
		# added cat properties to CMSPROPERTYDATA
		
		###########
		
		# add cat to CMSDOCUMENT
		$MYSQL "INSERT INTO CMSDOCUMENT (NODEID, PUBLISHED, DOCUMENTUSER, VERSIONID, TEXT, UPDATEDATE, TEMPLATEID, NEWEST) VALUES ($catId, b'1', 0, '$catVersionGuid', '${catName}', '${versionDateTime}', 1054, b'1');"
		# added cat to CMSDOCUMENT
	fi
	# finished processing category check
	
	###########
	
	# add product to UMBRACONODE
	sortOrder=$($MYSQL "SELECT MAX(SORTORDER) FROM UMBRACONODE WHERE NODEOBJECTTYPE='c66ba18e-eaf3-4cff-8a22-41b16d66a972' AND PARENTID=${catId};")
	sortOrder=$(($sortOrder+1))
	
	guid=$(r=( $(openssl rand 100000 | sha1sum) ); printf "%s${r[0]:0:8}-${r[0]:8:4}-${r[0]:12:4}-${r[0]:16:4}-${r[0]:20:12}\n")
	
	prodId=$($MYSQL "INSERT INTO UMBRACONODE (PARENTID, NODEUSER, LEVEL, PATH, SORTORDER, UNIQUEID, TEXT, NODEOBJECTTYPE) VALUES (${catId}, 0, 3, '', $sortOrder, '$guid', '$prodName', 'c66ba18e-eaf3-4cff-8a22-41b16d66a972'); SELECT LAST_INSERT_ID ();")
	$MYSQL "UPDATE UMBRACONODE SET PATH='-1,1050,${catId},${prodId}' WHERE ID=$prodId;"
	# product added to UMBRACONODE
	
	###########

	# setup version in CMSCONTENTVERSION
	prodVersionGuid=$(r=( $(openssl rand 100000 | sha1sum) ); printf "%s${r[0]:0:8}-${r[0]:8:4}-${r[0]:12:4}-${r[0]:16:4}-${r[0]:20:12}\n")
	$MYSQL "INSERT INTO CMSCONTENTVERSION (CONTENTID, VERSIONID, VERSIONDATE) VALUES ($prodId, '$prodVersionGuid', '$versionDateTime');"
	# finished setting up version in CMSCONTENTVERSION

	###########

	# add product to CMSCONTENT (1057 is the content type id for product pages)
	$MYSQL "INSERT INTO CMSCONTENT (NODEID, CONTENTTYPE) VALUES ($prodId, 1057);"
	# product added to CMSCONTENT

	###########
		
	# add product properties to CMSPROPERTYDATA
	$MYSQL "INSERT INTO CMSPROPERTYDATA (CONTENTNODEID, VERSIONID, PROPERTYTYPEID, DATANVARCHAR) VALUES ($prodId, '$prodVersionGuid', 34, '${prodName}');"
	$MYSQL "INSERT INTO CMSPROPERTYDATA (CONTENTNODEID, VERSIONID, PROPERTYTYPEID, DATAINT) VALUES ($prodId, '$prodVersionGuid', 35, $prodPrice);"
	$MYSQL "INSERT INTO CMSPROPERTYDATA (CONTENTNODEID, VERSIONID, PROPERTYTYPEID, DATANTEXT) VALUES ($prodId, '$prodVersionGuid', 36, '<ul><li>${prodDesc}</li></ul>');"
	$MYSQL "INSERT INTO CMSPROPERTYDATA (CONTENTNODEID, VERSIONID, PROPERTYTYPEID, DATAINT) VALUES ($prodId, '$prodVersionGuid', 37, $imgId);"
	# added product properties to CMSPROPERTYDATA
		
	###########
		
	# add product to CMSDOCUMENT
	$MYSQL "INSERT INTO CMSDOCUMENT (NODEID, PUBLISHED, DOCUMENTUSER, VERSIONID, TEXT, UPDATEDATE, TEMPLATEID, NEWEST) VALUES ($prodId, b'1', 0, '$prodVersionGuid', '${prodName}', '${versionDateTime}', 1055, b'1');"
	# added product to CMSDOCUMENT
	
	###########
	# IMPORT of products finished
	###########
	
	# clean up temp img file
	rm -f $imgTempPath
done

