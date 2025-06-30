
function ProjectAdd(tit,msg) {

    swal({
        title: tit ,
        text: "<h3>Saving Project... </h3><br />" + msg,
        type: "success",
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Okay",
        closeOnConfirm: false,
        html: true
           
    },
     function(){ 
        location.reload();
    });   
}


//Success Edit
function ProjectEdit(tit,msg) {

    swal({
        title: tit ,
        text: "<h5>Updating Project... </h5><br />" + msg,
        type: "success",
        confirmButtonColor: "#0F204B",
        confirmButtonText: "Okay",
        closeOnConfirm: false,
        html: true
           
    },
      function(){ 
            location.reload();
        }); 
     
}

function ActivityAdd(tit,msg) {

    swal({
        title: tit ,
        text: "<h3>Saving Activity... </h3><br />" + msg,
        type: "success",
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Okay",
        closeOnConfirm: false,
        html: true           
    },
    function(){ 
        location.reload();
    });   
}

function ActivityUpdate(tit,msg) {

    swal({
        title: tit ,
        text: "<h3>Updating Activity... </h3><br />" + msg,
        type: "success",
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Okay",
        closeOnConfirm: false,
        html: true           
    },
    function(){ 
        location.reload();
    });   
}



