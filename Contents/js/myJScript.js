// var allInputs = document.getElementsByTagName("input");
//        var last = "NameUnlikelyToBeUsedAsAnElementName";
//        alert("hello");
//        alert(allInputs.length);
//        var s = sender.id;

//        var parts = s.split("_");

//        var result = parts[parts.length - 2];

//        var divname = "Div_" + result;
//        
//        
//        for (i = 0; i < allInputs.length; i++) {
//            var input = allInputs[i];
//            if (input.name == last) continue; // if this object name is the same as the last checked radio, go to next iteration


//            // checks to see if any  one of  similarly named radiobuttons is checked 
//            else if (input.type == "radio") {
//                last = input.name;
//                // alert(last);
//                var radios = document.getElementsByName(input.name);

//                var radioSelected = false;
//                // alert(input.id);
//                // alert(sender.id);
//                // var s1 = sender.id;

//                var parts1 = s.split("_");

//                var result1 = parts1[parts1.length - 2];

//                var s2 = input.name;

//                var parts2 = s2.split("_");

//                var result2 = parts2[parts2.length - 2];
//                // alert('result1 ' + result1 + ' result2 ' + result2);
//                // alert('s2 ' + s2 + ' s ' + s);


//                //iterate over question options


//                // alert('ctl00_contentQuestionnaireBody_y' + divname);
//                //                                document.getElementById('ctl00_contentQuestionnaireBody_y' + divname).style.display = 'block';
//                //                                document.getElementById('ctl00_contentQuestionnaireBody_n' + divname).style.display = 'none';
//                animatedcollapse.addDiv('ctl00_contentQuestionnaireBody_y' + divname, 'fade=1,height=100px');
//                animatedcollapse.addDiv('ctl00_contentQuestionnaireBody_n' + divname, 'fade=1,height=100px');
//                alert('ctl00_contentQuestionnaireBody_y' + divname);
//            }
//        }
//        
//        animatedcollapse.init(); 