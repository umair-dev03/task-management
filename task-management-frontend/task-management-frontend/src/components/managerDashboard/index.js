import React, { useState } from "react";
import { Grid, GridColumn, GridToolbar } from '@progress/kendo-react-grid';
import { Button } from "@progress/kendo-react-buttons";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { fetchTasksPaginated } from "../../services/taskService";
import "./style.css";

function ManagerDashboard() {
  const [tasks, setTasks] = useState([]);
  const [allTasks, setAllTasks] = useState([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [searchEmployeeName, setSearchEmployeeName] = useState('');
  const [status, setStatus] = useState('');
  const [loading, setLoading] = useState(false);

  // Status confirmation modal state
  const [showStatusModal, setShowStatusModal] = useState(false);
  const [selectedTask, setSelectedTask] = useState(null);
  const [newStatus, setNewStatus] = useState('');
  const [submittingStatus, setSubmittingStatus] = useState(false);

  // Fetch paginated tasks
  React.useEffect(() => {
    async function loadTasks() {
      setLoading(true);
      try {
        const res = await fetchTasksPaginated({
          page,
          pageSize,
          searchEmployeeName: '', // Always fetch all data from API
          status
        });

        // Store all unfiltered tasks
        setAllTasks(res.items || []);
        setTasks(res.items || []);
        setTotalCount(res.totalCount || 0);
      } catch (err) {
        toast.error(err.message || "Failed to load tasks");
      } finally {
        setLoading(false);
      }
    }
    loadTasks();
  }, [page, pageSize, status]);

  // Separate useEffect for frontend filtering
  React.useEffect(() => {
    if (allTasks.length > 0) {
      let filteredTasks = allTasks;
      let filteredTotalCount = allTasks.length;

      if (searchEmployeeName && searchEmployeeName.trim()) {
        const searchTerm = searchEmployeeName.toLowerCase().trim();
        filteredTasks = allTasks.filter(task => {
          // Check if search term contains any letters or digits
          const hasLettersOrDigits = /[a-zA-Z0-9]/.test(searchTerm);

          if (!hasLettersOrDigits) return true; // If no letters/digits, show all

          // Search across multiple fields
          return (
            (task.title && task.title.toLowerCase().includes(searchTerm)) ||
            (task.employeeName && task.employeeName.toLowerCase().includes(searchTerm)) ||
            (task.status && task.status.toLowerCase().includes(searchTerm)) ||
            (task.hourWorked && task.hourWorked.toString().includes(searchTerm)) ||
            (task.date && task.date.toLowerCase().includes(searchTerm))
          );
        });

        filteredTotalCount = filteredTasks.length;
      }

      setTasks(filteredTasks);
      setTotalCount(filteredTotalCount);
    }
  }, [searchEmployeeName, allTasks]);

  // Handle status change confirmation
  const handleStatusChange = (task, newStatus) => {
    setSelectedTask(task);
    setNewStatus(newStatus);
    setShowStatusModal(true);
  };

  // Confirm status change
  const confirmStatusChange = async () => {
    if (!selectedTask || !newStatus) return;

    setSubmittingStatus(true);
    try {
      // Here you would call the API to update the task status
      // For now, we'll just update the local state
      setTasks(prevTasks =>
        prevTasks.map(task =>
          task.id === selectedTask.id
            ? { ...task, status: newStatus }
            : task
        )
      );

      toast.success(`Task status updated to ${newStatus}!`);
      setShowStatusModal(false);
      setSelectedTask(null);
      setNewStatus('');
    } catch (error) {
      toast.error("Failed to update task status");
    } finally {
      setSubmittingStatus(false);
    }
  };

  // Status cell renderer with buttons
  const StatusCell = (props) => {
    const { dataItem } = props;
    const currentStatus = dataItem.status || 'Pending';

    // Determine button colors based on current status
    const getApproveButtonColor = () => {
      if (currentStatus === 'Approved') return '#28a745'; // Green
      if (currentStatus === 'Rejected') return '#6c757d'; // Gray
      return '#6c757d'; // Gray for pending
    };

    const getRejectButtonColor = () => {
      if (currentStatus === 'Rejected') return '#dc3545'; // Red
      if (currentStatus === 'Approved') return '#6c757d'; // Gray
      return '#6c757d'; // Gray for pending
    };

    return (
      <td {...props.tdProps}>
        <div style={{
          display: 'flex',
          gap: '6px',
          justifyContent: 'center',
          alignItems: 'center'
        }}>
          <Button
            themeColor={currentStatus === 'Approved' ? 'success' : 'secondary'}
            size="small"
            onClick={() => handleStatusChange(dataItem, 'Approved')}
            style={{
              backgroundColor: getApproveButtonColor(),
              color: 'white',
              border: 'none',
              borderRadius: '4px',
              padding: '3px 8px',
              cursor: 'pointer',
              fontSize: '10px',
              fontWeight: '600',
              minWidth: '50px',
              height: '24px',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              textTransform: 'uppercase',
              letterSpacing: '0.5px'
            }}
          >
            Approve
          </Button>
          <Button
            themeColor={currentStatus === 'Rejected' ? 'error' : 'secondary'}
            size="small"
            onClick={() => handleStatusChange(dataItem, 'Rejected')}
            style={{
              backgroundColor: getRejectButtonColor(),
              color: 'white',
              border: 'none',
              borderRadius: '4px',
              padding: '3px 8px',
              cursor: 'pointer',
              fontSize: '10px',
              fontWeight: '600',
              minWidth: '50px',
              height: '24px',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              textTransform: 'uppercase',
              letterSpacing: '0.5px'
            }}
          >
            Reject
          </Button>
        </div>
      </td>
    );
  };

  return (
    <div className="manager-dashboard">
      <h2>Manager Dashboard</h2>

      {/* Status Confirmation Modal */}
      {showStatusModal && (
        <div
          style={{
            position: "fixed",
            top: 0,
            left: 0,
            width: "100vw",
            height: "100vh",
            background: "rgba(0,0,0,0.5)",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            zIndex: 1000,
            backdropFilter: "blur(4px)"
          }}
        >
          <div
            style={{
              background: "#fff",
              borderRadius: "16px",
              padding: "0",
              minWidth: "400px",
              maxWidth: "500px",
              width: "90%",
              boxShadow: "0 20px 60px rgba(0,0,0,0.3)",
              display: "flex",
              flexDirection: "column",
              position: "relative",
              overflow: "hidden"
            }}
          >
            {/* Modal Header */}
            <div style={{
              background: "linear-gradient(135deg, #667eea 0%, #764ba2 100%)",
              padding: "24px 32px",
              color: "white",
              position: "relative"
            }}>
              <h3 style={{
                margin: 0,
                fontSize: "24px",
                fontWeight: "600",
                display: "flex",
                alignItems: "center",
                gap: "12px",
                color: "white"
              }}>
                Confirm Task Status
              </h3>
              <button
                onClick={() => {
                  setShowStatusModal(false);
                  setSelectedTask(null);
                  setNewStatus('');
                }}
                style={{
                  position: "absolute",
                  top: "16px",
                  right: "16px",
                  background: "rgba(255,255,255,0.2)",
                  border: "none",
                  borderRadius: "50%",
                  width: "32px",
                  height: "32px",
                  color: "white",
                  fontSize: "18px",
                  cursor: "pointer",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  transition: "all 0.2s ease"
                }}
                onMouseEnter={(e) => e.target.style.background = "rgba(255,255,255,0.3)"}
                onMouseLeave={(e) => e.target.style.background = "rgba(255,255,255,0.2)"}
              >
                ✕
              </button>
            </div>

            {/* Modal Body */}
            <div style={{ padding: "32px" }}>
              <div style={{ display: "flex", flexDirection: "column", gap: "24px" }}>
                <div style={{ textAlign: "center" }}>
                  <p style={{ fontSize: "16px", color: "#374151", margin: "0 0 16px 0" }}>
                    Are you sure you want to change the status of task:
                  </p>
                  <p style={{
                    fontSize: "18px",
                    fontWeight: "600",
                    color: "#667eea",
                    margin: "0 0 8px 0",
                    padding: "12px",
                    backgroundColor: "#f8f9fa",
                    borderRadius: "8px"
                  }}>
                    "{selectedTask?.title}"
                  </p>
                  <p style={{ fontSize: "14px", color: "#6b7280", margin: "0" }}>
                    Employee: {selectedTask?.employeeName}
                  </p>
                </div>

                {/* Action Buttons */}
                <div style={{
                  display: "flex",
                  gap: "12px",
                  marginTop: "8px",
                  justifyContent: "center"
                }}>
                  <Button
                    type="button"
                    onClick={() => {
                      setShowStatusModal(false);
                      setSelectedTask(null);
                      setNewStatus('');
                    }}
                    themeColor="secondary"
                    size="medium"
                    style={{
                      padding: "12px 24px",
                      borderRadius: "8px",
                      fontSize: "14px",
                      fontWeight: "600",
                      minWidth: "100px",
                      transition: "all 0.2s ease"
                    }}
                  >
                    Cancel
                  </Button>
                  <Button
                    type="button"
                    onClick={confirmStatusChange}
                    disabled={submittingStatus}
                    themeColor={newStatus === 'Approved' ? 'success' : 'error'}
                    size="medium"
                    style={{
                      padding: "12px 24px",
                      borderRadius: "8px",
                      fontSize: "14px",
                      fontWeight: "600",
                      minWidth: "120px",
                      transition: "all 0.2s ease",
                      backgroundColor: newStatus === 'Approved' ? '#28a745' : '#dc3545',
                      color: 'white',
                      boxShadow: newStatus === 'Approved'
                        ? "0 4px 12px rgba(34, 197, 94, 0.3)"
                        : "0 4px 12px rgba(220, 53, 69, 0.3)"
                    }}
                  >
                    {submittingStatus ? (
                      <span style={{ display: "flex", alignItems: "center", gap: "8px" }}>
                        <span style={{ animation: "spin 1s linear infinite" }}>⏳</span>
                        Updating...
                      </span>
                    ) : (
                      <span style={{ display: "flex", alignItems: "center", gap: "8px" }}>
                        Confirm {newStatus}
                      </span>
                    )}
                  </Button>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}

      <div className="task-list">
        <div className="three-div-container">
          <div className="search-div">
            <input
              type="text"
              placeholder="Search tasks..."
              value={searchEmployeeName}
              onChange={e => {
                const value = e.target.value;
                setSearchEmployeeName(value);
                setPage(1);
              }}
              style={{
                padding: "12px 16px",
                borderRadius: "6px",
                border: "1px solid #ddd",
                width: "100%",
                height: "44px",
                fontSize: "14px",
                outline: "none",
                transition: "border-color 0.2s ease",
                boxSizing: "border-box",
                marginBottom: "0px"
              }}
              onFocus={(e) => e.target.style.borderColor = "#007bff"}
              onBlur={(e) => e.target.style.borderColor = "#ddd"}
            />
          </div>

          <div className="select-div">
            <select
              value={status}
              onChange={e => { setStatus(e.target.value); setPage(1); }}
              style={{
                padding: "12px 16px",
                borderRadius: "6px",
                border: "1px solid #ddd",
                width: "100%",
                height: "44px",
                fontSize: "14px",
                outline: "none",
                backgroundColor: "white",
                cursor: "pointer",
                transition: "border-color 0.2s ease",
                boxSizing: "border-box"
              }}
              onFocus={(e) => e.target.style.borderColor = "#007bff"}
              onBlur={(e) => e.target.style.borderColor = "#ddd"}
            >
              <option value="">All Statuses</option>
              <option value="Pending">Pending</option>
              <option value="Approved">Approved</option>
              <option value="Rejected">Rejected</option>
            </select>
          </div>
        </div>

        {/* Task List Title */}
        <div style={{
          display: "flex",
          justifyContent: "center",
          margin: "2rem 0"
        }}>
          <h3 style={{
            margin: 0,
            fontSize: "1.5rem",
            fontWeight: "600",
            color: "#333",
            textAlign: "center"
          }}>
            Task List
          </h3>
        </div>

        <div className="grid-container"
        >
          <Grid
            data={tasks}
            className="kendo-grid"
            resizable={true}
            sortable={true}
            pageable={{
              pageSizes: [5, 10, 20, 50],
              buttonCount: 5,
              info: true,
              previousNext: true,
              total: totalCount
            }}
            skip={(page - 1) * pageSize}
            take={pageSize}
            total={totalCount}
            onPageChange={e => {
              setPage(Math.floor(e.page.skip / e.page.take) + 1);
              setPageSize(e.page.take);
            }}
            loading={loading}
            dataItemKey="id"
          >
            <GridToolbar>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '8px 0' }}>
                <div style={{ fontSize: '14px', color: '#666' }}>
                  Showing {tasks.length} of {totalCount} tasks
                </div>
              </div>
            </GridToolbar>
            <GridColumn
              field="id"
              title="ID"
              headerClassName="grid-header-cell"
              width="4%"
            />
            <GridColumn
              field="employeeName"
              title="Employee Name"
              headerClassName="grid-header-cell"
            />
            <GridColumn
              field="title"
              title="Title"
              headerClassName="grid-header-cell"
            />
            <GridColumn
              field="date"
              title="Date"
              headerClassName="grid-header-cell"
              cells={{
                data: (props) => {
                  const date = props.dataItem.date;
                  if (!date) return <td {...props.tdProps}>-</td>;

                  try {
                    const dateObj = new Date(date);
                    const formattedDate = dateObj.toLocaleDateString('en-GB', {
                      day: '2-digit',
                      month: '2-digit',
                      year: 'numeric'
                    });
                    return <td {...props.tdProps}>{formattedDate}</td>;
                  } catch (error) {
                    return <td {...props.tdProps}>-</td>;
                  }
                }
              }}
            />
            <GridColumn
              field="hourWorked"
              title="Hours Worked"
              headerClassName="grid-header-cell"
            />
            <GridColumn
              field="status"
              title="Status"
              headerClassName="grid-header-cell"
              cells={{
                data: StatusCell
              }}
            />
          </Grid>
        </div>
      </div>
      <ToastContainer />
    </div>
  );
}

export default ManagerDashboard;
