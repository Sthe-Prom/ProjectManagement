
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


function ProjectAddError(tit,msg) {

    swal({
        title: tit ,
        text: '<div style="text-align: left;padding: 0 10px;">' + msg + '</div>',
        type: "error",
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Okay",
        closeOnConfirm: false,
        html: true           
    },
    );   
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

function ActivityAddError(tit,msg) {

    swal({
        title: tit,
        text: '<div style="text-align: left;padding: 0 10px;">' + msg + '</div>',
        type: "error",
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Okay",
        closeOnConfirm: true,
        html: true           
    },
   );   
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


function delUser(btn) {
    swal({
        title: "Are you sure?",
        text: "Are you sure you want to delete this user?",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes, delete user!",
        cancelButtonText: "No, cancel!"
    }, function (isConfirm) {

        if (!isConfirm) return;

        let form = btn.closest("form");
        let url = form.action;

        let formData = new FormData(form);

        fetch(url, {
            method: "POST",
            body: formData
        })
        .then(r => r.json())
        .then(data => {
            showToast(data.responseMessage, data.responseCode === 0);
        })
        .catch(err => {
            showToast("An error occurred.", false);
        });
    });
}

// function delUser(btn){
//     swal({
//         title: "Are you sure?",
//         text: "Are you sure you want to delete this user?",
//         type: "warning",
//         showCancelButton: true,
//         confirmButtonColor: "#DD6B55",
//         confirmButtonText: "Yes, delete user!",
//         cancelButtonText: "No, cancel!",
//         closeOnConfirm: false,
//         closeOnCancel: false
//     },
//         function (isConfirm) {
//             if (isConfirm) {
//                 btn.closest('form').submit();
//                 swal("Deleted!", "Selected user deleted.", "success");
//             } else {
//                 swal("Cancelled", "user not deleted:)", "error");
//             }
//         });    
     
// }





 //Filter()
        const tableSort = (tableId) => {
        const table = document.getElementById(tableId);
        if (!table) {
            console.error('Table not found');
            return;
        }

        const thead = table.querySelector("thead");
        const tbody = table.querySelector("tbody");
        if (!thead || !tbody) {
            console.error("Table must have <thead> and <tbody>");
            return;
        }

        const headers = thead.querySelectorAll("th");
        if (headers.length === 0) {
            console.error("No table headers found");
            return;
        }

        // Initialize sorting direction for each column
        const sortDirections = Array(headers.length).fill("asc");

        const getCellValue = (row, column) => {
            const cell = row.children[column];
            // Handle cases where cell is not found
            if (!cell) return '';
            const cellValue = cell.textContent.trim();
            return isNaN(cellValue) ? cellValue : parseFloat(cellValue);
        };

        const sortTableByColumn = (column, direction) => {
            const rows = Array.from(tbody.querySelectorAll("tr"));
            rows.sort((a, b) => {
                const cellA = getCellValue(a, column);
                const cellB = getCellValue(b, column);
                const order = direction === "asc" ? 1 : -1;
                if (typeof cellA === 'number' && typeof cellB === 'number') {
                    return order * (cellA - cellB);
                } else {
                    return order * cellA.toString().localeCompare(cellB);
                }
            });
            rows.forEach(row => tbody.appendChild(row));
        };

        const handleHeaderClick = (index) => {
            sortTableByColumn(index, sortDirections[index]);
            sortDirections[index] = sortDirections[index] === "asc" ? "desc" : "asc";
            updateHeaderClasses();
        };

        const updateHeaderClasses = () => {
            headers.forEach((header, index) => {
                header.classList.remove("sort-asc", "sort-desc");
                header.classList.add(sortDirections[index] === "asc" ? "sort-asc" : "sort-desc");
            });
        };

        // Attach click event listeners to each header cell
        headers.forEach((header, index) => {
            header.addEventListener("click", () => handleHeaderClick(index));
        });
    };

    // Initialize table sorting once the document is fully loaded
    document.addEventListener('DOMContentLoaded', () => {
        //tableSort("projectsTable");
    });




