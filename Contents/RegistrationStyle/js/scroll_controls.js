/*************************************************************************
    This code is from Dynamic Web Coding at dyn-web.com
    Copyright 2008 by Sharon Paine 
    See Terms of Use at www.dyn-web.com/business/terms.php
    regarding conditions under which you may use this code.
    This notice must be retained in the code as is!

    unobtrusive event handling for use with dw_scroll.js
*************************************************************************/

/////////////////////////////////////////////////////////////////////
// two ways to add style sheet for capable browsers
dw_writeStyleSheet = function(file) {
    document.write('<link rel="stylesheet" href="' + file + '" media="screen" />');
}

function dw_addLinkCSS(file) {
    if ( !document.createElement ) return;
    var el = document.createElement("link");
    el.setAttribute("rel", "stylesheet");
    el.setAttribute("type", "text/css");
    el.setAttribute("media", "screen");
    el.setAttribute("href", file);
    document.getElementsByTagName('head')[0].appendChild(el);
}
/////////////////////////////////////////////////////////////////////

// load_wn_lyr1 
// Why specify the id of the scroll area in this class name but not the others ?  
// I don't believe I have tested the addition of the horizontal ID that's needed when you using horizontal scrolling 
// load_wn_lyr2_t2
dw_scrollObj.prototype.setUpLoadLinks = function(controlsId) {
    if ( !document.getElementById || !document.getElementsByTagName ) return;
    var wndoId = this.id; var el = document.getElementById(controlsId); 
    var links = el.getElementsByTagName('a');
    var cls, new_cls, parts;
    for (var i=0; links[i]; i++) {
        cls = dw_scrollObj.get_DelimitedClass( links[i].className );
        parts = cls.split('_');
        if ( parts[0] == 'load' ) {
            // more checking here?
            
            new_cls = cls.replace( 'load_', '');
            links[i].className = links[i].className.replace(cls, new_cls);
            dw_Event.add( links[i], 'click', dw_scrollObj.initLayerLoad );
        }
    }
}

dw_scrollObj.prototype.setUpScrollControls = function(controlsId, autoHide, axis) {
    if ( !document.getElementById || !document.getElementsByTagName ) return;
    var wndoId = this.id; var el = document.getElementById(controlsId); 
    if ( autoHide ) {
        dw_scrollObj.handleControlVis(controlsId, wndoId, axis);
        dw_Scrollbar_Co.addEvent( this, 'on_load', function() { dw_scrollObj.handleControlVis(controlsId, wndoId, axis); } );
    }
    var wn = document.getElementById( wndoId );
    // support area too? later
    var links = el.getElementsByTagName('a');
    var cls, new_cls, parts, eType, eAlt, fn, x, y, dur;
    var re, dur_re = /^([\d]+)$/;
    
    // Reminder: doesn't work to set up anonymous functions here, passing arguments, because of closures !
    for (var i=0; links[i]; i++) {
         x = '', y = ''; // restore
        // Get first class with underscores
        cls = dw_scrollObj.get_DelimitedClass( links[i].className );
        parts = cls.split('_'); 
        eType = dw_scrollObj.getEv_FnType( parts[0] );
        switch ( eType ) {
            case 'mouseover' :
            case 'mousedown' :
            re = /^(mouseover|mousedown)_(up|down|left|right)(_[\d]+)?$/;
                // replace mouseover/mousedown in class name with wndoId (eg. wn_left_100)
                if ( re.test(cls) ) { 
                    new_cls = cls.replace( eType, wndoId);
                    links[i].className = links[i].className.replace(cls, new_cls);
                    eAlt = (eType == 'mouseover')? 'mouseout': 'mouseup';
                    dw_Event.add( links[i], eType, dw_scrollObj.initScrollMouse );
                    dw_Event.add( links[i], eAlt, dw_scrollObj.stopScrollMouse );
                    if ( eType == 'mouseover') {
                        dw_Event.add( links[i], 'mousedown', dw_scrollObj.increaseSpeed );
                        dw_Event.add( links[i], 'mouseup', dw_scrollObj.restoreDefaultSpeed );
                    }
                    dw_Event.add( links[i], 'click', 
                        function(e) { if (e && e.preventDefault) e.preventDefault(); return false; } ); 
                }
                continue;
    
            case 'scrollTo' :
                fn = 'scrollTo';
                re = /^(null|end|[\d]+)$/;
                x = re.test( parts[1] )? parts[1]: '';
                y = re.test( parts[2] )? parts[2]: '';
                dur = ( parts[3] && dur_re.test(parts[3]) )? parts[3]: null;
                break;
            case 'scrollBy':// scrollBy_m30_m40, scrollBy_null_m100, scrollBy_100_null
                fn = 'scrollBy';
                re = /^(([m]?[\d]+)|null)$/;
                x = re.test( parts[1] )? parts[1]: '';
                y = re.test( parts[2] )? parts[2]: '';
                dur = ( parts[3] && dur_re.test(parts[3]) )? parts[3]: null;
                break;
            case 'scrollToId': 
                new_cls = wndoId + '_' + links[i].className;
                links[i].className = links[i].className.replace(cls, new_cls);
                dw_Event.add( links[i], 'click', dw_scrollObj.scrollToId );
                continue;
            case 'click': 
                var o = dw_scrollObj.getClickParts(cls);
                fn = o.fn; x = o.x; y = o.y; dur = o.dur;
                break;
        }
        if ( x !== '' && y !== '' ) {
            new_cls = wndoId + '_' + fn + '_' + x + '_' + y + ( dur? '_' + dur: '');
            links[i].className = links[i].className.replace(cls, new_cls);
            dw_Event.add( links[i], 'click', dw_scrollObj.doOnclick );
        }
    }
}

// get info from className (e.g., click_down_by_100)
dw_scrollObj.getClickParts = function(cls) {
    var parts = cls.split('_');
    var re = /^(up|down|left|right)$/;
    var dir, fn, x, y, dur, ar;
    if ( ar = parts[1].match(re) ) { dir = ar[1]; }
    re = /^(to|by)$/; 
    ar = parts[2].match(re);
    fn = (ar[0] == 'to')? 'scrollTo': (ar[0] == 'by')? 'scrollBy': '';
    var val = parts[3]; // value on x or y axis
    if ( parts[4] ) { 
        dur = !isNaN( parts[4] )? parts[4]: null;
    }
    
    // no hyphens in classes. indicate a negative number with m - down_by_100 to scrollBy_0_m100
    if (dir) { // If direction is specified, value on one axis is implied 
        switch (fn) {
            case 'scrollBy' :
                re = /^([\d]+)$/;
                if ( !re.test( val ) ) {
                    x = ''; y = ''; break;
                }
                switch (dir) { // 0 for unspecified axis 
                    case 'up' : x = 0; y = val; break;
                    case 'down' : x = 0; y = 'm' + val; break;
                    case 'left' : x = val; y = 0; break;
                    case 'right' : x = 'm' + val; y = 0;
                 }
                break;
            case 'scrollTo' :
                re = /^(end|[\d]+)$/;
                if ( !re.test( val ) ) {
                    x = ''; y = ''; break;
                }
                switch (dir) { // null for unspecified axis 
                    case 'up' : x = null; y = val; break;
                    case 'down' : x = null; y = (val == 'end')? val: 'm' + val; break;
                    case 'left' : x = val; y = null; break;
                    case 'right' : x = (val == 'end')? val: 'm' + val; y = null;
                 } 
                break;
         }
    }
    return { fn: fn, x: x, y: y, dur: dur }
}

dw_scrollObj.getEv_FnType = function(str) {
    var re = /^(mouseover|mousedown|scrollBy|scrollTo|scrollToId|click)$/;
    if (re.test(str) ) {
        return str;
    }
    return '';
}

// return class name with underscores in it 
dw_scrollObj.get_DelimitedClass = function(cls) {
    if ( cls.indexOf('_') == -1 ) {
        return '';
    }
    var whitespace = /\s+/;
    if ( !whitespace.test(cls) ) {
        return cls;
    } else {
        var classes = cls.split(whitespace); 
        for(var i = 0; classes[i]; i++) { 
            if ( classes[i].indexOf('_') != -1 ) {
                return classes[i];
            }
        }
    }
}

dw_scrollObj.doOnclick = function(e) {
    var tgt = dw_scrollObj.getTargetLink(e);
    var cls = dw_scrollObj.get_DelimitedClass( tgt.className );
    var parts = cls.split('_');
    var wndoId = parts[0];  var fn = parts[1]; 
    var x = parts[2].replace('m', '-'); var y = parts[3].replace('m', '-'); 
    var dur = parts[4] || null;
    var wndo = dw_scrollObj.col[wndoId];
    if (x == 'end') { x = wndo.maxX; }
    if (y == 'end') { y = wndo.maxY; }
    if (x == 'null') { x = wndo.x; }
    if (y == 'null') { y = wndo.y; }
    x = parseInt(x); y = parseInt(y);
    if (fn == 'scrollBy') {
        wndo.initScrollByVals(x, y, dur);
    } else if (fn == 'scrollTo') {
        wndo.initScrollToVals(x, y, dur);
    }
    if (e && e.preventDefault) e.preventDefault();
    return false;
}

// wn_scrollToId_smile, wn_scrollToId_smile_100, wn_scrollToId_smile_lyr1_100
dw_scrollObj.scrollToId = function(e) {
    var dur;
    var tgt = dw_scrollObj.getTargetLink(e);
    var cls = dw_scrollObj.get_DelimitedClass( tgt.className );
    var parts = cls.split('_');
    var wndoId = parts[0];  var wndo = dw_scrollObj.col[wndoId];
    var el = document.getElementById( parts[2] );
    if (el) {
        if ( parts[3] ) {
            if ( isNaN(parts[3]) ) { // Check for and load the layer 
                var id = parts[3];
                if ( document.getElementById(id) && wndo.lyrId != id ) {
                    dw_scrollObj.col[wndoId].load(id);
                }
                dur = parts[4] && !isNaN(parts[4])? parts[4]: null;
            } else {
                dur = parts[3];
            }
        }
        var lyr = document.getElementById(wndo.lyrId);
        var x = dw_getLayerOffset(el, lyr, 'left');
        var y = dw_getLayerOffset(el, lyr, 'top');
        wndo.initScrollToVals(x, y, dur);
    }
    if (e && e.preventDefault) e.preventDefault();
    return false;
}

dw_scrollObj.increaseSpeed = function(e) {
    var wndoId = dw_scrollObj.getWndoIdFromClass(e);
    dw_scrollObj.col[wndoId].speed *= 3;
}

dw_scrollObj.restoreDefaultSpeed = function(e) {
    var wndoId = dw_scrollObj.getWndoIdFromClass(e);
    dw_scrollObj.col[wndoId].speed = dw_scrollObj.prototype.speed;
    if (e && e.preventDefault) e.preventDefault();
    return false;
}

dw_scrollObj.initScrollMouse = function(e) {
    var tgt = dw_scrollObj.getTargetLink(e);
    var cls = dw_scrollObj.get_DelimitedClass( tgt.className );
    var parts = cls.split('_'); // eg. wn_down_100
    var wndoId = parts[0];  var dir = parts[1];  
    var speed = parts[2] || null; 
    var deg = dir == 'up'? 90: dir == 'down'? 270: dir == 'left'? 180: dir == 'right'? 0: null;
    if ( deg != null ) {
        dw_scrollObj.col[wndoId].initScrollVals(deg, speed);
    }
}

dw_scrollObj.stopScrollMouse = function(e) {
    var wndoId = dw_scrollObj.getWndoIdFromClass(e);
    dw_scrollObj.col[wndoId].ceaseScroll();
}

dw_scrollObj.initLayerLoad = function(e) {
    var tgt = dw_scrollObj.getTargetLink(e);
    var cls = dw_scrollObj.get_DelimitedClass( tgt.className );
    //alert(cls)
    var parts = cls.split('_'); 
    var wndoId = parts[0]; var lyrId = parts[1]; var horizId = parts[2]? parts[2]: null;
    dw_scrollObj.col[wndoId].load(lyrId, horizId);
    if (e && e.preventDefault) e.preventDefault();
    return false;
}

dw_scrollObj.getWndoIdFromClass = function(e) {
    var tgt = dw_scrollObj.getTargetLink(e);
    var cls = dw_scrollObj.get_DelimitedClass( tgt.className );
    return cls.slice(0, cls.indexOf('_') );
}

dw_scrollObj.getTargetLink = function(e) {
    dw_Event.DOMit(e);
    var tgt = e.target;
    do {
        if ( tgt.tagName == 'A' ) {
            return tgt;
        }
    } while ( tgt = tgt.parentNode)
    return '';
}

dw_scrollObj.handleControlVis = function(controlsId, wndoId, axis) {
    var wndo = dw_scrollObj.col[wndoId];
    var el = document.getElementById(controlsId);
    if ( ( axis == 'v' && wndo.maxY > 0 ) || ( axis == 'h' && wndo.maxX > 0 ) ) {
        el.style.visibility = 'visible';
    } else {
        el.style.visibility = 'hidden';
    }
}