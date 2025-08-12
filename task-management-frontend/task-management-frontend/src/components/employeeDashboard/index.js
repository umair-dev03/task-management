import React, { useState } from "react";
import { Grid, GridColumn, GridToolbar } from '@progress/kendo-react-grid';
import { createTask, fetchTasksPaginated, updateTask, deleteTask } from "../../services/taskService";
import { Button } from "@progress/kendo-react-buttons";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import "./style.css";

function EmployeeDashboard() {
  const [tasks, setTasks] = useState([]);
  const [allTasks, setAllTasks] = useState([]); // Store all unfiltered tasks
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [searchEmployeeName, setSearchEmployeeName] = useState('');
  const [status, setStatus] = useState('');
  const [loading, setLoading] = useState(false);
  const [newTaskIds, setNewTaskIds] = useState(new Set());


  const remove = async (dataItem) => {
    const confirmDelete = window.confirm(`Are you sure you want to delete the task "${dataItem.title}"?`);
    if (confirmDelete) {
      try {
        await deleteTask(dataItem.id);

        // Remove from local state immediately
        setTasks(prevTasks => prevTasks.filter(task => task.id !== dataItem.id));

        // Also refresh from server to ensure consistency
        const res = await fetchTasksPaginated({
          page,
          pageSize,
          searchEmployeeName: '', // Always fetch all data from API
          status
        });

        // If current page is empty and we're not on page 1, go to previous page
        if (res.items.length === 0 && page > 1) {
          setPage(page - 1);
        } else {
          setTasks(res.items || []);
          setTotalCount(res.totalCount || 0);
        }

        toast.success("Task deleted successfully!");
      } catch (error) {
        toast.error(error.message || "Failed to delete task");
      }
    }
  };



  const MyCommandCell = (props) => (
    <td {...props.tdProps}>
      <div style={{
        display: 'flex',
        gap: '6px',
        justifyContent: 'center',
        alignItems: 'center',
        padding: '4px'
      }}>
        <Button
          themeColor={'primary'}
          onClick={() => handleEditClick(props.dataItem)}
          size="small"
          style={{
            backgroundColor: '#007bff',
            color: 'white',
            border: 'none',
            borderRadius: '4px',
            padding: '3px 6px',
            cursor: 'pointer',
            fontSize: '10px',
            fontWeight: '600',
            minWidth: '40px',
            height: '24px',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            textTransform: 'uppercase',
            letterSpacing: '0.5px'
          }}
        >
          Edit
        </Button>
        <Button
          onClick={() => remove(props.dataItem)}
          size="small"
          style={{
            backgroundColor: '#dc3545',
            color: 'white',
            border: 'none',
            borderRadius: '4px',
            padding: '3px 6px',
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
          Delete
        </Button>
      </div>
    </td>
  );

  // Fetch paginated tasks
  React.useEffect(() => {
    async function loadTasks() {
      setLoading(true);
      try {
        console.log('🔍 Loading tasks from server...');
        const res = await fetchTasksPaginated({
          page,
          pageSize,
          searchEmployeeName: '', // Always fetch all data from API
          status
        });

        console.log('🔍 Server response on load:', res);
        console.log('🔍 Current local state before update:', { allTasks, tasks });

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

        // Filter tasks based on search term
        filteredTasks = allTasks.filter(task => {
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

  // Modal state
  const [showModal, setShowModal] = useState(false);
  const [isEditMode, setIsEditMode] = useState(false);
  const [editingTaskId, setEditingTaskId] = useState(null);
  const [taskForm, setTaskForm] = useState({
    title: "",
    date: "",
    hourWorked: "",
  });
  const [submitting, setSubmitting] = useState(false);

  // Handle modal input changes
  const handleModalChange = (e) => {
    const { name, value } = e.target;
    setTaskForm((prev) => ({
      ...prev,
      [name]: name === "hourWorked" ? (value === "" ? "" : Number(value)) : value,
    }));
  };

  // Open modal for creating new task
  const handleCreateClick = () => {
    setIsEditMode(false);
    setEditingTaskId(null);
    setTaskForm({
      title: "",
      date: "",
      hourWorked: "",
    });
    setShowModal(true);
  };

  // Open modal for editing existing task
  const handleEditClick = (task) => {
    setIsEditMode(true);
    setEditingTaskId(task.id);

    // Format date for date input (without timezone)
    let formattedDate = "";
    if (task.date) {
      try {
        // Extract the date part directly from the original string to avoid timezone issues
        const datePart = task.date.split('T')[0];
        formattedDate = datePart + 'T00:00:00';
      } catch (error) {
        formattedDate = new Date().toISOString().split('T')[0] + 'T00:00:00';
      }
    }

    setTaskForm({
      title: task.title || "",
      date: formattedDate,
      hourWorked: task.hourWorked !== null && task.hourWorked !== undefined ? task.hourWorked : "",
    });
    setShowModal(true);
  };

  // Handle form submission (create or update)
  const handleSubmitTask = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      // Prepare task data according to API structure
      const taskData = {
        title: taskForm.title,
        date: taskForm.date ? `${taskForm.date.split('T')[0]}T00:00:00` : new Date().toISOString().split('T')[0] + 'T00:00:00',
        hourWorked: taskForm.hourWorked === "" || taskForm.hourWorked === null || taskForm.hourWorked === undefined ? 0 : Number(taskForm.hourWorked),
      };

      // For editing, include additional fields that might be required by the API
      if (isEditMode) {
        // Find the original task to get additional fields
        const originalTask = allTasks.find(task => task.id === editingTaskId);
        if (originalTask) {
          // Include fields that shouldn't change during edit
          taskData.employeeName = originalTask.employeeName;
          taskData.status = originalTask.status;
        }
      }

      if (isEditMode) {
        // Update existing task using PUT API
        const updatedTask = await updateTask(editingTaskId, taskData);
        toast.success("Task updated successfully!", { position: "top-right" });

        // Update both allTasks and tasks arrays with the latest server value (same pattern as manager dashboard)
        setAllTasks(prev =>
          prev.map(task => (task.id === editingTaskId ? { ...task, ...updatedTask } : task))
        );
        setTasks(prev =>
          prev.map(task => (task.id === editingTaskId ? { ...task, ...updatedTask } : task))
        );
      } else {
        // Create new task using POST API
        const created = await createTask(taskData);
        toast.success("Task created successfully!", { position: "top-right" });

        // Add the new task to both allTasks and tasks state (same pattern as manager dashboard)
        setAllTasks(prev => [created, ...prev]);
        setTasks(prev => [created, ...prev]);
        setTotalCount(prev => prev + 1);

        // Mark this task as newly created for highlighting
        setNewTaskIds(prev => new Set([...prev, created.id]));

        // Remove highlight after 5 seconds
        setTimeout(() => {
          setNewTaskIds(prev => {
            const newSet = new Set(prev);
            newSet.delete(created.id);
            return newSet;
          });
        }, 5000);

      }

      // Reset modal state
      setShowModal(false);
      setTaskForm({ title: "", date: "", hourWorked: "" });
      setIsEditMode(false);
      setEditingTaskId(null);
    } catch (error) {
      toast.error(error.message || "Operation failed. Please try again.", { position: "top-right" });
    } finally {
      setSubmitting(false);
    }
  };


  return (
    <div className="employee-dashboard">
      {/* Logout Button - Above Heading */}
      <div style={{
        display: 'flex',
        justifyContent: 'flex-end',
        marginBottom: '1rem'
      }}>
        <Button
          onClick={() => {
            localStorage.removeItem('user');
            window.location.href = '/';
          }}
          themeColor="error"
          size="small"
          style={{
            width: "150px",
            padding: "6px 12px",
            borderRadius: "4px",
            fontSize: "12px",
            fontWeight: "600",
            backgroundColor: '#dc3545',
            color: 'white',
            border: 'none',
            cursor: 'pointer',
            transition: "all 0.2s ease"
          }}
        >
          Logout
        </Button>
      </div>

      {/* Dashboard Title */}
      <h2>Employee Dashboard</h2>

      {/* Modal for creating/editing a task */}
      {showModal && (
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
                {isEditMode ? "Edit Task" : "Create New Task"}
              </h3>
              <button
                onClick={() => {
                  setShowModal(false);
                  setTaskForm({ title: "", date: "", hourWorked: "" });
                  setIsEditMode(false);
                  setEditingTaskId(null);
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
              <form onSubmit={handleSubmitTask} style={{ display: "flex", flexDirection: "column", gap: "24px" }}>
                {/* Title Field */}
                <div style={{ display: "flex", flexDirection: "column", gap: "8px" }}>
                  <label style={{
                    fontSize: "14px",
                    fontWeight: "600",
                    color: "#374151",
                    display: "flex",
                    alignItems: "center",
                    gap: "8px"
                  }}>
                    Task Title
                  </label>
                  <input
                    type="text"
                    name="title"
                    value={taskForm.title}
                    onChange={handleModalChange}
                    required
                    placeholder="Enter task title..."
                    style={{
                      width: "100%",
                      padding: "14px 16px",
                      borderRadius: "8px",
                      border: "2px solid #e5e7eb",
                      fontSize: "16px",
                      outline: "none",
                      transition: "all 0.2s ease",
                      boxSizing: "border-box"
                    }}
                    onFocus={(e) => {
                      e.target.style.borderColor = "#667eea";
                      e.target.style.boxShadow = "0 0 0 3px rgba(102, 126, 234, 0.1)";
                    }}
                    onBlur={(e) => {
                      e.target.style.borderColor = "#e5e7eb";
                      e.target.style.boxShadow = "none";
                    }}
                  />
                </div>

                {/* Date Field */}
                <div style={{ display: "flex", flexDirection: "column", gap: "8px" }}>
                  <label style={{
                    fontSize: "14px",
                    fontWeight: "600",
                    color: "#374151",
                    display: "flex",
                    alignItems: "center",
                    gap: "8px"
                  }}>
                    Date & Time
                  </label>
                  <div >
                    <input
                      type="date"
                      value={taskForm.date ? taskForm.date.split('T')[0] : ''}
                      onChange={(e) => {
                        const dateValue = e.target.value;
                        setTaskForm(prev => ({
                          ...prev,
                          date: dateValue ? `${dateValue}T00:00:00` : ""
                        }));
                      }}
                      style={{
                        padding: "14px 16px",
                        borderRadius: "8px",
                        border: "2px solid #e5e7eb",
                        fontSize: "16px",
                        outline: "none",
                        transition: "all 0.2s ease",
                        boxSizing: "border-box",
                        backgroundColor: "white",
                        color: "#374151",
                        height: "48px"
                      }}
                      onFocus={(e) => {
                        e.target.style.borderColor = "#667eea";
                        e.target.style.boxShadow = "0 0 0 3px rgba(102, 126, 234, 0.1)";
                      }}
                      onBlur={(e) => {
                        e.target.style.borderColor = "#e5e7eb";
                        e.target.style.boxShadow = "none";
                      }}
                    />

                  </div>
                </div>

                {/* Hours Worked Field */}
                <div style={{ display: "flex", flexDirection: "column", gap: "8px" }}>
                  <label style={{
                    fontSize: "14px",
                    fontWeight: "600",
                    color: "#374151"
                  }}>
                    Hours Worked
                  </label>
                  <input
                    type="text"
                    name="hourWorked"
                    value={taskForm.hourWorked || ""}
                    onChange={handleModalChange}
                    required
                    placeholder="Enter hours worked..."
                    style={{
                      width: "100%",
                      padding: "14px 16px",
                      borderRadius: "8px",
                      border: "2px solid #e5e7eb",
                      fontSize: "16px",
                      outline: "none",
                      boxSizing: "border-box"
                    }}
                  />
                </div>

                {/* Action Buttons */}
                <div style={{
                  display: "flex",
                  gap: "12px",
                  marginTop: "8px",
                  justifyContent: "flex-end"
                }}>
                  <Button
                    type="button"
                    onClick={() => {
                      setShowModal(false);
                      setTaskForm({ title: "", date: "", hourWorked: "" });
                      setIsEditMode(false);
                      setEditingTaskId(null);
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
                    type="submit"
                    disabled={submitting}
                    themeColor="success"
                    size="medium"
                    style={{
                      padding: "12px 24px",
                      borderRadius: "8px",
                      fontSize: "14px",
                      fontWeight: "600",
                      minWidth: "120px",
                      transition: "all 0.2s ease",
                      boxShadow: "0 4px 12px rgba(34, 197, 94, 0.3)"
                    }}
                  >
                    {submitting ? (
                      <span style={{ display: "flex", alignItems: "center", gap: "8px" }}>
                        <span style={{ animation: "spin 1s linear infinite" }}>⏳</span>
                        {isEditMode ? "Updating..." : "Creating..."}
                      </span>
                    ) : (
                      <span style={{ display: "flex", alignItems: "center", gap: "8px" }}>
                        {isEditMode ? "Update Task" : "Create Task"}
                      </span>
                    )}
                  </Button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}

      <div className="task-list">
        {/* Search and Filter Section */}
        <>
          <div className="employee-three-div-container">
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
                <option value="Completed">Completed</option>
                <option value="InProgress">In Progress</option>
              </select>
            </div>

            <div className="button-div">
              <Button
                onClick={handleCreateClick}
                themeColor="success"
                size="medium"
                style={{
                  padding: "12px 16px",
                  borderRadius: "6px",
                  fontSize: "14px",
                  fontWeight: "600",
                  width: "100%",
                  height: "44px",
                  boxShadow: "0 2px 4px rgba(0,0,0,0.1)",
                  transition: "all 0.2s ease",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center"
                }}
              >
                + Create Task
              </Button>
            </div>
          </div>
        </>

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
        <div className="grid-container" >
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
            rowRender={(row, props) => {
              const isNewTask = newTaskIds.has(props.dataItem.id);
              return React.cloneElement(row, {
                className: isNewTask ? 'new-task' : row.props.className
              });
            }}
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
              field="title"
              title="Title"
              headerClassName="grid-header-cell"
            />
            <GridColumn
              field="employeeName"
              title="Employee Name"
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
              title="Actions"
              headerClassName="grid-header-cell"
              cells={{
                data: MyCommandCell
              }}
            />
          </Grid>
        </div>
      </div>
      <ToastContainer />
    </div >
  );
}

export default EmployeeDashboard;
