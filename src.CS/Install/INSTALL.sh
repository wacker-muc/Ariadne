#! /bin/sh
#------------------------------------------------------------------------
# Installation script, for Linux.
#------------------------------------------------------------------------

case "$0" in
    *UNINSTALL | *UNINSTALL.sh )
	task="U"		# uninstall
    ;;
    * )
	task="I"		# install
    ;;
esac

usage() {
    if [ -n "$1" ] ; then
	echo "[EE] $1"
    fi
    P1="install" ; P2="into"
    if [ "$task" = "U" ] ; then
	P1="uninstall" ; P2="from"
    fi
    cat <<EOF
usage: $0 ~/.local
   or: $0 ~
   or: $0 /usr/local -- requires sudo permissions
which will $P1 the application $P2 the given path's lib and bin
subdirectories.
EOF
    exit 1
}

if [ -z "$1" ] ; then
    usage
fi

if [ -z "`which mono`" -a "$task" = "I" ] ; then
    usage "You first need to install the mono runtime environment."
fi

LIB="$1/lib"
BIN="$1/bin"
APP="$1/share/applications"
MAN="$1/man"

SUDO=""
RM="rm -r"
CP="cp -a"
MKDIR="mkdir -p"
CHMOD="chmod -R"
case "$1" in
    /* )
	echo "[I] An installation in $1 requires sudo persission."
	SUDO="sudo"
	RM="sudo $RM"
	CP="sudo $CP"
	MKDIR="sudo $MKDIR"
	CHMOD="sudo $CHMOD"
	;;
esac

# Consistency check: some directories should already exist.
if [ "$task" = "I" ] ; then
    for d in "$BIN" "$APP" ; do
	if [ ! -d "$d" ] ; then
	    usage "directory doesn't exist: $d"
##	elif [ ! -w "$d" ] ; then
##	    usage "directory is not writable: $d"
	fi
    done
fi

# Other directories will be created now.
$MKDIR "$LIB" "$MAN/man1"

cp_dir() {
    if [ "$task" = "I" ] ; then
	$MKDIR "$2" || usage "cannot create directory: $2"
	echo installing directory: "$2"
	all_files="$all_files $2"
	$CP --no-preserve=ownership "$1/." "$2/"
	$CHMOD g-w "$2"
    else
	echo removing directory: "$2"
	$RM "$2"
    fi
}

# Determine the location of the binary files
BINSRC=".."
if [ ! -f "$BINSRC/Ariadne.exe" ] ; then
    BINSRC="../SWA.Ariadne.App/bin/Release"
fi
if [ ! -f "$BINSRC/Ariadne.exe" ] ; then
    BINSRC="../SWA.Ariadne.App/bin/Debug"
fi

# Make shared Screenshots directory writable for all users
cp_dir "$BINSRC" "$LIB/Ariadne"
if [ -n "$SUDO" ] ; then
    $MKDIR      "$LIB/Ariadne/Screenshots"
    $CHMOD 1777 "$LIB/Ariadne/Screenshots"
fi

cp_file() {
    if [ "$task" = "I" ] ; then
	echo installing file: "$2"
	all_files="$all_files $2"
	cat "$1" | sed -e "s|<LIB>|$LIB|" \
		       -e "s|\( *\)<UNINSTALL>|\1$RM $all_files\n\1$UNINSTALL_XSCREENSAVER|" \
	    | $SUDO tee "$2" > /dev/null
	$SUDO touch -r "$1" "$2"
	$CHMOD $3 "$2"
    else
	echo removing file: "$2"
	$RM "$2"
    fi
}

# TODO: use "here" documents
cp_file "./ariadne-interactive.desktop.tmpl" "$APP/ariadne-interactive.desktop" 644
cp_file "./ariadne-fullscreen.desktop.tmpl" "$APP/ariadne-fullscreen.desktop" 644
cp_file "./ariadne-options.desktop.tmpl" "$APP/ariadne-options.desktop" 644
cp_file "./ariadne.1.tmpl" "$MAN/man1/ariadne.1" 644

# Patch a user file, will not use sudo.
# Parameters: file, pattern, awk-program
patch_file() {
    if [ ! -f "$1" ] ; then
	echo "[!] file doesn't exist, cannot be patched: $1"
	return
    fi
    UNINSTALL_XSCREENSAVER="cp -a $1 $1.bak ; grep -v '$2' $1.bak > $1"
    if [ "$task" = "I" ] ; then
	if [ -n "`grep "$2" $1`" ] ; then
	    echo "no patch required: $1"
	    return
	fi
	echo patching file: "$1"
	cp  -a $1 $1.bak
	awk -e "$3" $1.bak > $1 || \
	    echo "[E] patch failed! -- check $1 and $1.bak"
    else
	echo patching file: "$1"
	sh -c "$UNINSTALL_XSCREENSAVER"
    fi
}

# Patch xscreensaver configuration.
# Find the first empty line after the 'programs:' section
# and insert one additional program line.
prog=`cat <<EOF
/^programs:/ { state = "prog" }
/^$/ {
    if (state == "prog") {
	print "\t\t\t\t$BIN/ariadne \\\\\\\\n\\\\\\\\"
	state = "tail"
    }
}
{ print }
EOF`
patch_file $HOME/.xscreensaver "$BIN/ariadne" "$prog"

# ... and last:
cp_file "./ariadne.sh.tmpl" "$BIN/ariadne" 755

# Check if our manpage is on the MANPATH
if [ "$task" = "I" -a -n "$MANPATH" -a -z "`echo $MANPATH | grep $MAN`" ] ; then
    echo ""
    echo "[!] Please add $MAN to your MANPATH."
    echo ""
fi

echo "done"
