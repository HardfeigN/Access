var datatable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    datatable = $('#inquiryDataTable').DataTable({
        "ajax": {
            "url":"/inquiry/GetInquiryList"
        },
        "columns": [
            {"data":"id","width":"10%"},
            {"data":"fullName","width":"15%"},
            {"data":"phoneNumber","width":"15%"},
            {"data": "email", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="">
                            <a href="/Inquiry/Details/${data}" class"a-inquiry-edit-btn" >
                                <i class="fa-solid fa-pen-alt a-inquiry-edit-btn-icon"></i>
                            </a>
                        </div>
                    
                    `;
                },
                "width":"5%"
            }
        ]
    });
}